using AspNetCoreGeneratedDocument;
using ChatApp.Data;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Repository
{
    public class UserRepository : IUserRepository
    {

        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context; 
        }
        public async Task<User> CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);    
            await _context.SaveChangesAsync();  
            return user;
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return
                await _context.Users.SingleOrDefaultAsync(user=>
            user.UserName == userName);
        }
        public async Task<bool> ExistAsync(string userName)
        {
            return
                await _context.Users.AnyAsync(user=>
            user.UserName == userName);
        }

        public async Task<IEnumerable<User>> SearchAsync(string Keyword)
        {

            return await _context.Users
                 .Where(e => e.UserName.Contains(Keyword))
                 .Select(u => new User { Id = u.Id, UserName = u.UserName })
                 .ToListAsync();
        }
    }
}
