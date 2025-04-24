using Microsoft.EntityFrameworkCore;
using AgriConnect.Models;

namespace AgriConnect.Utils
{
    public class AgriConnectContext : DbContext
    {
        public AgriConnectContext(DbContextOptions<AgriConnectContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        
    }
}
