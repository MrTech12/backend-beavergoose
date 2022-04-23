using LinkMicroservice.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinkMicroservice.Data
{
    public class LinkContext : DbContext
    {
        public LinkContext()
        {

        }

        public LinkContext(DbContextOptions<LinkContext> options) : base(options)
        {

        }
        public DbSet<Link> Links { get; private set; } // Entity set AKA database table

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Change table names to singular variant.
            modelBuilder.Entity<Link>().ToTable("Link");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfiguration conf = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());
                string connectionString = conf.GetConnectionString("LinkContext");

                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}
