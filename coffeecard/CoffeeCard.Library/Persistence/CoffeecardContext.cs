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
        public DbSet<Product> Products { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<Programme> Programmes { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<ProductUserGroup> ProductUserGroups { get; set; }
        public DbSet<WebhookConfiguration> WebhookConfigurations { get; set; }

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

            // Use Enum to Int for UserGroups
            var userGroupIntConverter = new EnumToNumberConverter<UserGroup, int>();

            modelBuilder.Entity<User>()
                .Property(u => u.UserGroup)
                .HasConversion(userGroupIntConverter);

            modelBuilder.Entity<User>().Property(u => u.UserState).HasConversion<string>();
            
            modelBuilder.Entity<ProductUserGroup>()
                .Property(pug => pug.UserGroup)
                .HasConversion(userGroupIntConverter);

            modelBuilder.Entity<WebhookConfiguration>()
                .Property(w => w.Status)
                .HasConversion<string>();
            
            if (_environmentSettings.EnvironmentType != EnvironmentType.Production)
            {
                SeedData(modelBuilder);
            }
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Programmes
            modelBuilder.Entity<Programme>().HasData(
                new Programme() {Id = 1, ShortName = "SWU", FullName = "BSc Software Development"},
                new Programme() {Id = 2, ShortName = "GBI", FullName = "BSc Global Business Informatics"},
                new Programme() {Id = 3, ShortName = "BDDIT", FullName = "BSc Digital Design and Interactive Technologies"},
                new Programme() {Id = 4, ShortName = "KDDIT", FullName = "MSc Digital Design and Interactive Technologies"},
                new Programme() {Id = 5, ShortName = "DIM", FullName = "MSc Digital Innovation and Management"},
                new Programme() {Id = 6, ShortName = "E-BUSS", FullName = "MSc E-Business"},
                new Programme() {Id = 7, ShortName = "GAMES/DT", FullName = "MSc Games - Design and Theory"},
                new Programme() {Id = 8, ShortName = "GAMES/Tech", FullName = "MSc Games - Technology"},
                new Programme() {Id = 9, ShortName = "CS", FullName = "MSc Computer Science"},
                new Programme() {Id = 10, ShortName = "SDT", FullName = "MSc Software Development (Design)"},
                new Programme() {Id = 11, ShortName = "Employee", FullName = "Employee"},
                new Programme() {Id = 12, ShortName = "Other", FullName = "Other"},
                new Programme() {Id = 13, ShortName = "DS", FullName = "BSc Data Science"}
            );
            
            // Products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Price = 80, NumberOfTickets = 10, Name = "Filter Coffee", Description = "Used for filter coffee brewed with fresh ground coffee", ExperienceWorth = 10, Visible = true },
                new Product { Id = 2, Price = 150, NumberOfTickets = 10, Name = "Espresso Based", Description = "Used for specialities like espresso, cappuccino, caffe latte, cortado, americano and chai latte", ExperienceWorth = 150, Visible = true }
            );

            modelBuilder.Entity<ProductUserGroup>().HasData(
                new ProductUserGroup {ProductId = 1, UserGroup = UserGroup.Customer},
                new ProductUserGroup {ProductId = 2, UserGroup = UserGroup.Customer}
            );
        }
    }
}