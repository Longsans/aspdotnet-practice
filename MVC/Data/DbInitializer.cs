using Common.Models;
using Common.Utilities;

namespace Practice.Data
{
    public static class DbInitializer
    {
        public static void Init(WebAppContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
                return;

            User defaultUser = new()
            {
                Username = "Long",
                Password = Encrypter.EncryptSHA256("`123")
            };
            context.Users.Add(defaultUser);
            context.SaveChanges();
        }
    }
}
