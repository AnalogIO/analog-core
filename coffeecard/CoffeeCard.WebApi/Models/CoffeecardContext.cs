using CoffeeCard.Common.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CoffeeCard.WebApi.Models
{
    public class CoffeeCardContext : DbContext
    {
        private readonly DatabaseSettings _databaseSettings;

        public CoffeeCardContext(DbContextOptions<CoffeeCardContext> options, DatabaseSettings databaseSettings)
            : base(options)
        {
            _databaseSettings = databaseSettings;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<Programme> Programmes { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<ProductUserGroup> ProductUserGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(_databaseSettings.SchemaName);

            modelBuilder.Entity<ProductUserGroup>()
                .HasKey(pug => new
                {
                    pug.ProductId,
                    pug.UserGroup
                });
        }
    }
}