using CoffeeCard.Common.Configuration;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CoffeeCard.Library.Persistence
{
    public class CoffeeCardContext : DbContext
    {
        private readonly DatabaseSettings _databaseSettings;

        private readonly EnvironmentSettings _environmentSettings;

        public CoffeeCardContext(DbContextOptions<CoffeeCardContext> options, DatabaseSettings databaseSettings, EnvironmentSettings environmentSettings)
            : base(options)
        {
            _databaseSettings = databaseSettings;
            _environmentSettings = environmentSettings;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PosPurhase> PosPurchases { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<Programme> Programmes { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<ProductUserGroup> ProductUserGroups { get; set; }
        public DbSet<WebhookConfiguration> WebhookConfigurations { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }

        public DbSet<MenuItemProduct> MenuItemProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(_databaseSettings.SchemaName);

            // Setup PUG compound primary key
            modelBuilder.Entity<ProductUserGroup>()
                .HasKey(pug => new
                {
                    pug.ProductId,
                    pug.UserGroup
                });

            modelBuilder.Entity<MenuItemProduct>()
                .HasKey(mip => new { mip.MenuItemId, mip.ProductId });

            modelBuilder.Entity<MenuItem>()
                .HasMany(mi => mi.AssociatedProducts)
                .WithMany(p => p.EligibleMenuItems)
                .UsingEntity<MenuItemProduct>();

            // Use Enum to Int for UserGroups
            var userGroupIntConverter = new EnumToNumberConverter<UserGroup, int>();
            // Use Enum to String for PurchaseTypes
            var purchaseTypeStringConverter = new EnumToStringConverter<PurchaseType>();

            modelBuilder.Entity<User>()
                .Property(u => u.UserGroup)
                .HasConversion(userGroupIntConverter);

            modelBuilder.Entity<Purchase>()
                .Property(p => p.Type)
                .HasConversion(purchaseTypeStringConverter);

            modelBuilder.Entity<User>().Property(u => u.UserState).HasConversion<string>();

            modelBuilder.Entity<ProductUserGroup>()
                .Property(pug => pug.UserGroup)
                .HasConversion(userGroupIntConverter);

            modelBuilder.Entity<WebhookConfiguration>()
                .Property(w => w.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Purchase>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Ticket>()
                .HasOne<User>(t => t.Owner)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.OwnerId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}