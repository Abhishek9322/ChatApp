using ChatApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> option):base(option)
        {
            
        }

        public DbSet<Message> messages { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
    }
}
