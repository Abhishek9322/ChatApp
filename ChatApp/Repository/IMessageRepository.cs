using ChatApp.Models;

namespace ChatApp.Repository
{
    public interface IMessageRepository
    {
        Task<Message> SaveEncryptedMessageAsync(Message message);
        Task<IEnumerable<Message>> GetRecentAsync(string username ,int count = 50);
    }
}
