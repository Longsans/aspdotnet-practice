using Practice.Models;

namespace Practice.Services
{
    public interface IUserService
    {
        User? FindByUsername(string username);
        User? FindByUserCredentials(string username, string password);
        bool ValidateUsername(string username);
        Task CreateUser(User user);
        Task UpdateUserInfo(User user);
        Task UpdateUserPassword(User update);
        Task DeleteByUsername(string username);
    }
}
