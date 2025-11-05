using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Collections.Concurrent;
using System.Text.Json;

namespace ChatApp.Models
{
    [Authorize]
    public class ChatHub:Hub
    {

        private static readonly ConcurrentDictionary<string, HashSet<string>> _userConnections = new();
        public override async Task OnConnectedAsync()
        {
            var username = Context.User?.Identity?.Name;
            var connectionId = Context.ConnectionId;

            if (!string.IsNullOrWhiteSpace(username))
            {
                await Groups.AddToGroupAsync(connectionId, username);

                _userConnections.AddOrUpdate(
                    username,
                    _ => new HashSet<string> { connectionId },
                    (_, set) =>
                    {
                        set.Add(connectionId);
                        return set;
                    });
            }
            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId=Context.ConnectionId;
            foreach(var kv in _userConnections)
            {
                if (kv.Value.Remove(connectionId))
                {
                    if(kv.Value.Count == 0)
                    {
                        _userConnections.TryRemove(kv.Key, out _);
                    }

                    await Groups.RemoveFromGroupAsync(connectionId, kv.Key);
                }
            }
            await base.OnDisconnectedAsync(exception);

        }

        public async Task Register(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return;
            }

            var connectionId = Context.ConnectionId;

            await Groups.AddToGroupAsync(connectionId, username);

            _userConnections.AddOrUpdate(
                username, _ => new HashSet<string> { connectionId },
                (_, set) =>
                {
                    set.Add(connectionId);
                    return set;
                });
        }
        public async Task Unregister(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return;

            var connectionId = Context.ConnectionId;
            await Groups.RemoveFromGroupAsync(connectionId, username);

            if(_userConnections.TryGetValue(username, out var set))
            {
                set.Remove(connectionId);

                if(set.Count == 0)
                    _userConnections.TryRemove(username ,out _);
            }
        }


    }
}
