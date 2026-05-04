using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
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

            var purchase1 = PurchaseBuilder
                .Simple()
                .WithId(1)
                .WithProductName("Large")
                .WithProductId(largeProduct.Id)
                .WithProduct(largeProduct)
                .WithPrice(20)
                .WithNumberOfTickets(1)
                .WithOrderId("order-1")
                .WithStatus(PurchaseStatus.Completed)
                .WithType(PurchaseType.Free)
                .WithPurchasedById(user.Id)
                .WithPurchasedBy(user)
                .WithDateCreated(currentTime.AddDays(-1))
                .Build();
            var purchase2 = PurchaseBuilder
                .Simple()
                .WithId(2)
                .WithProductName("Small")
                .WithProductId(smallProduct.Id)
                .WithProduct(smallProduct)
                .WithPrice(20)
                .WithNumberOfTickets(1)
                .WithOrderId("order-2")
                .WithStatus(PurchaseStatus.Completed)
                .WithType(PurchaseType.Free)
                .WithPurchasedById(user.Id)
                .WithPurchasedBy(user)
                .WithDateCreated(currentTime.AddDays(-2))
                .Build();
            var purchase3 = PurchaseBuilder
                .Simple()
                .WithId(3)
                .WithProductName("Large")
                .WithProductId(largeProduct.Id)
                .WithProduct(largeProduct)
                .WithPrice(20)
                .WithNumberOfTickets(1)
                .WithOrderId("order-3")
                .WithStatus(PurchaseStatus.Completed)
                .WithType(PurchaseType.Free)
                .WithPurchasedById(user.Id)
                .WithPurchasedBy(user)
                .WithDateCreated(currentTime.AddDays(-3))
                .Build();
            var purchase4 = PurchaseBuilder
                .Simple()
                .WithId(4)
                .WithProductName("Small")
                .WithProductId(fancyProduct.Id)
                .WithProduct(fancyProduct)
                .WithPrice(20)
                .WithNumberOfTickets(1)
                .WithOrderId("order-4")
                .WithStatus(PurchaseStatus.Completed)
                .WithType(PurchaseType.Free)
                .WithPurchasedById(otherUser.Id)
                .WithPurchasedBy(otherUser)
                .WithDateCreated(currentTime.AddDays(-1))
                .Build();

            var tickets = new List<Ticket>
            {
                TicketBuilder
                    .Simple()
                    .WithId(1)
                    .WithDateCreated(currentTime.AddDays(-3))
                    .WithDateUsed(currentTime.AddDays(-3))
                    .WithProductId(largeProduct.Id)
                    .WithStatus(TicketStatus.Used)
                    .WithOwnerId(user.Id)
                    .WithOwner(user)
                    .WithPurchaseId(purchase1.Id)
                    .WithPurchase(purchase1)
                    .WithUsedOnMenuItemId(latte.Id)
                    .WithUsedOnMenuItem(latte)
                    .Build(),
                TicketBuilder
                    .Simple()
                    .WithId(2)
                    .WithDateCreated(currentTime.AddDays(-2))
                    .WithDateUsed(currentTime.AddDays(-2))
                    .WithProductId(smallProduct.Id)
                    .WithStatus(TicketStatus.Used)
                    .WithOwnerId(user.Id)
                    .WithOwner(user)
                    .WithPurchaseId(purchase2.Id)
                    .WithPurchase(purchase2)
                    .WithUsedOnMenuItemId(americano.Id)
                    .WithUsedOnMenuItem(americano)
                    .Build(),
                TicketBuilder
                    .Simple()
                    .WithId(3)
                    .WithDateCreated(currentTime.AddDays(-1))
                    .WithDateUsed(currentTime.AddDays(-1))
                    .WithProductId(largeProduct.Id)
                    .WithStatus(TicketStatus.Used)
                    .WithOwnerId(user.Id)
                    .WithOwner(user)
                    .WithPurchaseId(purchase3.Id)
                    .WithPurchase(purchase3)
                    .WithUsedOnMenuItemId(latte.Id)
                    .WithUsedOnMenuItem(latte)
                    .Build(),
                TicketBuilder
                    .Simple()
                    .WithId(4)
                    .WithDateCreated(currentTime)
                    .WithDateUsed(currentTime)
                    .WithProductId(fancyProduct.Id)
                    .WithStatus(TicketStatus.Used)
                    .WithOwnerId(otherUser.Id)
                    .WithOwner(otherUser)
                    .WithPurchaseId(purchase4.Id)
                    .WithPurchase(purchase4)
                    .WithUsedOnMenuItemId(espresso.Id)
                    .WithUsedOnMenuItem(espresso)
                    .Build(),
                TicketBuilder
                    .Simple()
                    .WithId(5)
                    .WithDateCreated(currentTime)
                    .WithDateUsed(currentTime)
                    .WithProductId(fancyProduct.Id)
                    .WithStatus(TicketStatus.Used)
                    .WithOwnerId(otherUser.Id)
                    .WithOwner(otherUser)
                    .WithPurchaseId(purchase4.Id)
                    .WithPurchase(purchase4)
                    .WithUsedOnMenuItemId(espresso.Id)
                    .WithUsedOnMenuItem(espresso)
                    .Build(),
            };

            context.Purchases.AddRange(purchase1, purchase2, purchase3, purchase4);
            context.Tickets.AddRange(tickets);
            await context.SaveChangesAsync();

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(provider => provider.UtcNow()).Returns(currentTime);

            var statisticsService = new StatisticsService(context, dateTimeProvider.Object);

            // Act
            var result = (await statisticsService.GetQuickStatsAsync(user)).ToList();

            // Assert
            Assert.Equal(4, result.Count);

            var totalDrinks = Assert.Single(result.Where(stat => stat.Key == "total-drinks-user"));
            Assert.Equal(3, totalDrinks.Value);
            Assert.Null(totalDrinks.SupportingText);

            var drinksToday = Assert.Single(
                result.Where(stat => stat.Key == "global-drinks-today")
            );
            Assert.Equal(2, drinksToday.Value);
            Assert.Null(drinksToday.SupportingText);

            var favouriteDrink = Assert.Single(result.Where(stat => stat.Key == "favourite-drink"));
            Assert.Equal(2, favouriteDrink.Value);
            Assert.Equal("Latte", favouriteDrink.SupportingText);

            var drinksThisWeek = Assert.Single(
                result.Where(stat => stat.Key == "drinks-this-week-user")
            );
            Assert.Equal(3, drinksThisWeek.Value);
            Assert.Null(drinksThisWeek.SupportingText);
        }
    }
}
