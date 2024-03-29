﻿using LinkMicroservice.DBMigration.Entities;
using LinkMicroservice.DBMigration.Helpers;
using Microsoft.EntityFrameworkCore;

namespace LinkMicroservice.DBMigration.Data
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
                optionsBuilder.UseNpgsql(RetrieveConnectionStringHelper.GetConnectionString());
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}
