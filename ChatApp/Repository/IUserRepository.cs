using ChatApp.Models;

namespace ChatApp.Repository
{
    public interface IUserRepository
    {
        Task<User?>GetByUserNameAsync(string userName);
        Task<User>CreateAsync(User user);
        Task<IEnumerable<User>> SearchAsync(string Keyword);
        Task<bool> ExistAsync(string userName);
    }
}
