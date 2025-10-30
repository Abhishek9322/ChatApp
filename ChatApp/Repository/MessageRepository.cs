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
             await _context.messages.AddAsync(message);  
             await _context.SaveChangesAsync();
             return message;
        }
        public async Task<IEnumerable<Message>> GetRecentAsync(int count = 50)
        {
          return await _context.messages
                .OrderByDescending(d => d.CreatedAt)
                .Take(count)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

        }
    }
}
