using ChatApp.Data;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;
        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Message> SaveEncryptedMessageAsync(Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            ValidateMessage(message);

            message.CreatedAt = message.CreatedAt == default
                ? DateTime.UtcNow
                : message.CreatedAt;

            try
            {
                await _context.messages.AddAsync(message);
                await _context.SaveChangesAsync();
                return message;
            }
            catch(DbUpdateException ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"Database update failed: {innerMessage}", ex);
            }
        }

        private static void ValidateMessage(Message message)
        {
            var misssingFields = new List<string>();

            if (string.IsNullOrWhiteSpace(message.Sender))
                misssingFields.Add(nameof(message.Sender));

            if (string.IsNullOrWhiteSpace(message.Recipient))
                misssingFields.Add(nameof(message.Recipient));

            if (string.IsNullOrWhiteSpace(message.CipherTextBase64))
                misssingFields.Add(nameof(message.CipherTextBase64));


            if (misssingFields.Any())
            {
                throw new InvalidOperationException(
                    $"Messsage valid faild. missaing or empty fiels:{string.Join(",", misssingFields)}");
            }
        }


        public async Task<IEnumerable<Message>> GetRecentAsync( string username, int count = 50)
        {
          return await _context.messages
                .Where(m=>m.Sender==username || m.Recipient==username)
                .OrderByDescending(d => d.CreatedAt)
                .Take(count)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

        }
    }
}
