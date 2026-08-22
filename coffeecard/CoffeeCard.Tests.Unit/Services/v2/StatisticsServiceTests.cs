using System;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.v2.Statistics;
using CoffeeCard.Models.Entities;
using CoffeeCard.Tests.Common.Builders;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services.v2
{
    public class StatisticsServiceTests
    {
        [Fact(DisplayName = "GetQuickStatsAsync returns the four quick stats")]
        public async Task GetQuickStatsAsyncReturnsFourQuickStats()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(GetQuickStatsAsyncReturnsFourQuickStats)
            );

            var databaseSettings = new DatabaseSettings { SchemaName = "test" };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test,
            };

            await using var context = new CoffeeCardContext(
                builder.Options,
                databaseSettings,
                environmentSettings
            );

            var user = UserBuilder.DefaultCustomer().WithId(1).Build();

            var otherUser = UserBuilder.DefaultCustomer().WithId(2).Build();

            context.Users.AddRange(user, otherUser);

            var currentTime = new DateTime(2026, 5, 3, 12, 0, 0, DateTimeKind.Utc);

            var latte = MenuItemBuilder.Simple().WithId(1).WithName("Latte").Build();
            var americano = MenuItemBuilder.Simple().WithId(2).WithName("Americano").Build();
            var espresso = MenuItemBuilder.Simple().WithId(3).WithName("Espresso").Build();

            context.MenuItems.AddRange(latte, americano, espresso);

            var largeProduct = ProductBuilder.Simple().WithId(1).WithName("Large").Build();
            var smallProduct = ProductBuilder.Simple().WithId(2).WithName("Small").Build();
            var fancyProduct = ProductBuilder.Simple().WithId(3).WithName("Fancy").Build();

            context.Products.AddRange(largeProduct, smallProduct, fancyProduct);

            var purchases = PurchaseBuilder.Simple().Build(4);

            purchases[0].Product = largeProduct;
            purchases[0].PurchasedBy = user;

            purchases[1].Product = smallProduct;
            purchases[1].PurchasedBy = user;

            purchases[2].Product = largeProduct;
            purchases[2].PurchasedBy = user;

            purchases[3].Product = fancyProduct;
            purchases[3].PurchasedBy = otherUser;

            var tickets = TicketBuilder.Simple().WithStatus(TicketStatus.Used).Build(5);

            tickets[0].DateUsed = currentTime.AddDays(-3);
            tickets[0].Owner = user;
            tickets[0].Purchase = purchases[0];
            tickets[0].UsedOnMenuItem = latte;

            tickets[1].DateUsed = currentTime.AddDays(-2);
            tickets[1].Owner = user;
            tickets[1].Purchase = purchases[1];
            tickets[1].UsedOnMenuItem = americano;

            tickets[2].DateUsed = currentTime.AddDays(-1);
            tickets[2].Owner = user;
            tickets[2].Purchase = purchases[2];
            tickets[2].UsedOnMenuItem = latte;

            tickets[3].DateUsed = currentTime;
            tickets[3].Owner = otherUser;
            tickets[3].Purchase = purchases[3];
            tickets[3].UsedOnMenuItem = espresso;

            tickets[4].DateUsed = currentTime;
            tickets[4].Owner = otherUser;
            tickets[4].Purchase = purchases[3];
            tickets[4].UsedOnMenuItem = espresso;

            context.Purchases.AddRange(purchases);
            context.Tickets.AddRange(tickets);
            await context.SaveChangesAsync();

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(provider => provider.UtcNow()).Returns(currentTime);

            var statisticsService = new StatisticsService(context, dateTimeProvider.Object);

            // Act
            var result = (await statisticsService.GetQuickStatsAsync(user)).ToList();

            // Assert
            Assert.Equal(4, result.Count);

            var totalDrinks = Assert.Single(
                result.Where(stat => stat.Key == QuickStatType.TotalDrinks)
            );
            Assert.Equal(3, totalDrinks.Value);
            Assert.Null(totalDrinks.SupportingText);

            var drinksToday = Assert.Single(
                result.Where(stat => stat.Key == QuickStatType.DrinksToday)
            );
            Assert.Equal(2, drinksToday.Value);
            Assert.Null(drinksToday.SupportingText);

            var favouriteDrink = Assert.Single(
                result.Where(stat => stat.Key == QuickStatType.FavouriteDrink)
            );
            Assert.Equal(2, favouriteDrink.Value);
            Assert.Equal("Latte", favouriteDrink.SupportingText);

            var drinksThisWeek = Assert.Single(
                result.Where(stat => stat.Key == QuickStatType.DrinksThisWeek)
            );
            Assert.Equal(3, drinksThisWeek.Value);
            Assert.Null(drinksThisWeek.SupportingText);
        }
    }
}
