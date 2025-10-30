using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Models
{
    public class ChatHub:Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
    }
}
