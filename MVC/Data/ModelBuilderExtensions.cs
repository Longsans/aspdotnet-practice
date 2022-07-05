using Microsoft.EntityFrameworkCore;
using Practice.Models;
using Practice.Utilities;

namespace Practice.Data
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder model)
        {
            model.Entity<User>()
                .HasData(
                    new User
                    {
                        Username = "Long",
                        Password = Encrypter.EncryptSHA256("`123")
                    }
                );
        }
    }
}
