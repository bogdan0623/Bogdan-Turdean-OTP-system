using Microsoft.EntityFrameworkCore;
using OTPBackend.Models;

namespace OTPBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {            
        }

        public DbSet<User> Users { get; set; } = default!;

        public DbSet<UserToken> UserTokens { get; set; } = default!;

        public DbSet<UserOtp> UserOtps { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity => { entity.HasIndex(e => e.Email).IsUnique(); });
        }
    }
}
