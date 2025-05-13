using AgriConnectPlatform.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AgriConnectPlatform.Data
{
    public class AgriConnectContext : IdentityDbContext
    {
        public AgriConnectContext(DbContextOptions<AgriConnectContext> options)
            : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Farmer> Farmers { get; set; }
    }
}
