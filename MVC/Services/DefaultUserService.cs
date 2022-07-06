using Practice.Models;
using Practice.Data;
using Practice.Utilities;
using Microsoft.EntityFrameworkCore;

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
            return _context.Users
                .FirstOrDefault(x => x.Username == username);
        }

        public User? FindWithContactByUsername(string username)
        {
            return _context.Users
                .Include(u => u.Contact)
                .FirstOrDefault(x => x.Username == username);
        }

        public User? FindWithContactByUsernameForDisplay(string username)
        {
            return _context.Users
                .Include(u => u.Contact)
                .AsNoTracking()
                .FirstOrDefault(x => x.Username == username);
        }

        public User? FindByUserCredentials(string username, string password)
        {
            var user = FindByUsername(username);
            if (user?.Password == Encrypter.EncryptSHA256(password))
                return user;

            return null;
        }

        public bool ValidateUsername(string username)
        {
            var user = FindByUsername(username);
            return user == null;
        }

        public async Task UpdateUserInfo(User update)
        {
            var user = FindByUsername(update.Username);
            if (user == null)
                throw new InvalidOperationException("User not found");

            user.FirstName = update.FirstName;
            user.LastName = update.LastName;
            user.Email = update.Email;
            user.Age = update.Age;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserPassword(User update)
        {
            var user = FindByUsername(update.Username);
            if (user == null)
                throw new InvalidOperationException("User not found");

            user.Password = Encrypter.EncryptSHA256(update.Password);
            await _context.SaveChangesAsync();
        }

        public Contact? FindContactByUsername(string username)
        {
            return _context.Contacts
                .SingleOrDefault(c => c.UserUsername == username);
        }

        public async Task AddContact(Contact contact)
        {
            var existing = FindContactByUsername(contact.UserUsername);
            if (existing != null)
            {
                throw new InvalidOperationException("User contact already exists");
            }
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateContact(Contact update)
        {
            var existing = FindContactByUsername(update.UserUsername);
            if (existing == null)
            {
                throw new InvalidOperationException("User contact doesn't exist");
            }
            existing.Phone = update.Phone;
            existing.Address = update.Address;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteContactByUsername(string username)
        {
            var existing = FindContactByUsername(username);

            if (existing == null)
            {
                throw new InvalidOperationException("User contact doesn't exist");
            }
            _context.Contacts.Remove(existing);
            await _context.SaveChangesAsync();
        }

        public async Task AddOrUpdateContactIfExists(Contact contact)
        {
            var existing = FindContactByUsername(contact.UserUsername);
            if (existing == null)
            {
                await AddContact(contact);
            }
            else
            {
                await UpdateContact(contact);
            }
        }
    }
}
