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

            var swu = new Programme
            {
                Id = 1,
                ShortName = "SWU",
                FullName = "Software Development",
                SortPriority = 1,
            };
            var ds = new Programme
            {
                Id = 2,
                ShortName = "DS",
                FullName = "Data Science",
                SortPriority = 2,
            };

            var user = new User
            {
                Id = 1,
                Name = "User1",
                Email = "user1@itu.dk",
                Password = "password",
                Salt = "salt",
                DateCreated = new DateTime(2025, 1, 1),
                IsVerified = true,
                PrivacyActivated = false,
                UserGroup = UserGroup.Customer,
                UserState = UserState.Active,
                ProgrammeId = swu.Id,
                Programme = swu,
            };

            var otherUser = new User
            {
                Id = 2,
                Name = "User2",
                Email = "user2@itu.dk",
                Password = "password",
                Salt = "salt",
                DateCreated = new DateTime(2025, 1, 1),
                IsVerified = true,
                PrivacyActivated = false,
                UserGroup = UserGroup.Customer,
                UserState = UserState.Active,
                ProgrammeId = ds.Id,
                Programme = ds,
            };

            context.Programmes.AddRange(swu, ds);
            context.Users.AddRange(user, otherUser);

            var currentTime = new DateTime(2026, 5, 3, 12, 0, 0, DateTimeKind.Utc);

            var latte = new MenuItem { Id = 1, Name = "Latte" };
            var americano = new MenuItem { Id = 2, Name = "Americano" };
            var espresso = new MenuItem { Id = 3, Name = "Espresso" };

            context.MenuItems.AddRange(latte, americano, espresso);

            var purchases = new List<Purchase>
            {
                new Purchase
                {
                    Id = 1,
                    ProductName = "Large",
                    ProductId = 1,
                    Price = 20,
                    NumberOfTickets = 1,
                    OrderId = "order-1",
                    Status = PurchaseStatus.Completed,
                    Type = PurchaseType.Free,
                    PurchasedById = user.Id,
                    PurchasedBy = user,
                    DateCreated = currentTime.AddDays(-1),
                },
                new Purchase
                {
                    Id = 2,
                    ProductName = "Small",
                    ProductId = 2,
                    Price = 20,
                    NumberOfTickets = 1,
                    OrderId = "order-2",
                    Status = PurchaseStatus.Completed,
                    Type = PurchaseType.Free,
                    PurchasedById = user.Id,
                    PurchasedBy = user,
                    DateCreated = currentTime.AddDays(-2),
                },
                new Purchase
                {
                    Id = 3,
                    ProductName = "Large",
                    ProductId = 1,
                    Price = 20,
                    NumberOfTickets = 1,
                    OrderId = "order-3",
                    Status = PurchaseStatus.Completed,
                    Type = PurchaseType.Free,
                    PurchasedById = user.Id,
                    PurchasedBy = user,
                    DateCreated = currentTime.AddDays(-3),
                },
                new Purchase
                {
                    Id = 4,
                    ProductName = "Small",
                    ProductId = 3,
                    Price = 20,
                    NumberOfTickets = 1,
                    OrderId = "order-4",
                    Status = PurchaseStatus.Completed,
                    Type = PurchaseType.Free,
                    PurchasedById = otherUser.Id,
                    PurchasedBy = otherUser,
                    DateCreated = currentTime.AddDays(-1),
                },
            };

            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = 1,
                    DateCreated = currentTime.AddDays(-3),
                    DateUsed = currentTime.AddDays(-3),
                    ProductId = 1,
                    Status = TicketStatus.Used,
                    OwnerId = user.Id,
                    Owner = user,
                    PurchaseId = purchases[0].Id,
                    Purchase = purchases[0],
                    UsedOnMenuItemId = latte.Id,
                    UsedOnMenuItem = latte,
                },
                new Ticket
                {
                    Id = 2,
                    DateCreated = currentTime.AddDays(-2),
                    DateUsed = currentTime.AddDays(-2),
                    ProductId = 2,
                    Status = TicketStatus.Used,
                    OwnerId = user.Id,
                    Owner = user,
                    PurchaseId = purchases[1].Id,
                    Purchase = purchases[1],
                    UsedOnMenuItemId = americano.Id,
                    UsedOnMenuItem = americano,
                },
                new Ticket
                {
                    Id = 3,
                    DateCreated = currentTime.AddDays(-1),
                    DateUsed = currentTime.AddDays(-1),
                    ProductId = 1,
                    Status = TicketStatus.Used,
                    OwnerId = user.Id,
                    Owner = user,
                    PurchaseId = purchases[2].Id,
                    Purchase = purchases[2],
                    UsedOnMenuItemId = latte.Id,
                    UsedOnMenuItem = latte,
                },
                new Ticket
                {
                    Id = 4,
                    DateCreated = currentTime,
                    DateUsed = currentTime,
                    ProductId = 3,
                    Status = TicketStatus.Used,
                    OwnerId = otherUser.Id,
                    Owner = otherUser,
                    PurchaseId = purchases[3].Id,
                    Purchase = purchases[3],
                    UsedOnMenuItemId = espresso.Id,
                    UsedOnMenuItem = espresso,
                },
                new Ticket
                {
                    Id = 5,
                    DateCreated = currentTime,
                    DateUsed = currentTime,
                    ProductId = 3,
                    Status = TicketStatus.Used,
                    OwnerId = otherUser.Id,
                    Owner = otherUser,
                    PurchaseId = purchases[3].Id,
                    Purchase = purchases[3],
                    UsedOnMenuItemId = espresso.Id,
                    UsedOnMenuItem = espresso,
                },
            };

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
