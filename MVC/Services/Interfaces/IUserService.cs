using Common.Models;

namespace Practice.Services
{
    public interface IUserService
    {
        User? FindByUsername(string username);
        User? FindWithContactByUsername(string username);
        User? FindWithContactByUsernameForDisplay(string username);
        User? FindByUserCredentials(string username, string password);
        bool ValidateUsername(string username);
        Task CreateUser(User user);
        Task UpdateUserInfo(User update);
        Task UpdateUserPassword(User update);
        Task DeleteByUsername(string username);

        Contact? FindContactByUsername(string username);
        Task AddContact(Contact contact);
        Task UpdateContact(Contact update);
        Task AddOrUpdateContactIfExists(Contact contact);
        Task DeleteContactByUsername(string username);
    }
}
