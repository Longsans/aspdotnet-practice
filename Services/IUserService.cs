using Practice.Models;

namespace Practice.Services
{
    public interface IUserService
    {
        User? FindByUsername(string username);
        Task CreateUser(User user);
        Task UpdateUserInfo(User user);
        Task UpdateUserPassword(User update);
        Task DeleteByUsername(string username);
    }
}
