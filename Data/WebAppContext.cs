using Practice.Models;
using Microsoft.EntityFrameworkCore;

namespace Practice.Data
{
    public class WebAppContext : DbContext
    {
        public WebAppContext(DbContextOptions<WebAppContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
