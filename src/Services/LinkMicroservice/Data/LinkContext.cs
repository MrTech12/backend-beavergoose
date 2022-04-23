using LinkMicroservice.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinkMicroservice.Data
{
    public class LinkContext : DbContext
    {
        public LinkContext(DbContextOptions<LinkContext> options) : base(options)
        {

        }
        public DbSet<Link> Links { get; set; } // Entity set AKA database table

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Change table names to singular variant.
            modelBuilder.Entity<Link>().ToTable("Link");
        }
    }
}
