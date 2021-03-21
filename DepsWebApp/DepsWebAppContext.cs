using System;
using DepsWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DepsWebApp
{
    public class DepsWebAppContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public DepsWebAppContext(DbContextOptions<DepsWebAppContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .ToTable("Accounts");
        }
    }
}
