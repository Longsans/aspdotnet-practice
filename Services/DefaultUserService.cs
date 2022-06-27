using Practice.Models;
using Practice.Data;
using Practice.Utilities;

namespace Practice.Services
{
    public class DefaultUserService : IUserService
    {
        private readonly WebAppContext _context;

        public DefaultUserService(WebAppContext webContext)
        {
            _context = webContext;
        }

        public async Task CreateUser(User user)
        {
            user.Password = Encrypter.EncryptSHA256(user.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByUsername(string username)
        {
            _context.Users.Remove(new User {Username = username});
            await _context.SaveChangesAsync();
        }

        public User? FindByUsername(string username)
        {
            return _context
                    .Users
                    .FirstOrDefault(x => x.Username == username);
        }

        public async Task UpdateUserInfo(User update)
        {
            var user = FindByUsername(update.Username);
            if (user == null)
                throw new Exception("User not found");

            user.Email = update.Email;
            user.Age = update.Age;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserPassword(User update)
        {
            var user = FindByUsername(update.Username);
            if (user == null)
                throw new Exception("User not found");

            user.Password = Encrypter.EncryptSHA256(update.Password);
            await _context.SaveChangesAsync();
        }
    }
}
