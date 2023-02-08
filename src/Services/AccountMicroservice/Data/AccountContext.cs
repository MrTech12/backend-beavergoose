using Common.Configuration.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AccountMicroservice.Data
{
    public class AccountContext : IdentityDbContext<IdentityUser>
    {
        public AccountContext()
        {

        }

        public AccountContext(DbContextOptions<AccountContext> options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = ConnectionStringHelper.GetConnectionString("AccountContext");

                optionsBuilder.UseNpgsql(connectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}
