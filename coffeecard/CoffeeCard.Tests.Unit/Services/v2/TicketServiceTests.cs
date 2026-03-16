using System;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Models.Entities;
using CoffeeCard.Tests.Common.Builders;
using Moq;
using Xunit;
using TicketService = CoffeeCard.Library.Services.v2.TicketService;

namespace CoffeeCard.Tests.Unit.Services.v2
{
    public class TicketServiceTests : BaseUnitTests
    {
        [Fact(
            DisplayName = "GetGroupedTicketsAsync returns owned products and includes menu items"
        )]
        public async Task GetGroupedTicketsAsync_ReturnsGroupedTicketsWithMenuItems()
        {
            // Arrange
            var user = UserBuilder.DefaultCustomer().WithName("Test User").Build();

            var cappuccino = MenuItemBuilder
                .Simple()
                .WithName("Cappuccino")
                .WithActive(true)
                .Build();

            var latte = MenuItemBuilder.Simple().WithName("Latte").WithActive(false).Build();

            var hiddenOwnedProduct = ProductBuilder
                .Simple()
                .WithName("Owned Hidden Clip Card")
                .WithVisible(false)
                .WithProductUserGroup([new ProductUserGroup { UserGroup = UserGroup.Manager }])
                .WithEligibleMenuItems([cappuccino, latte])
                .Build();

            var availableProduct = ProductBuilder
                .Simple()
                .WithName("Available Product")
                .WithDescription("Visible and eligible for customer")
                .WithVisible(true)
                .WithProductUserGroup([new ProductUserGroup { UserGroup = UserGroup.Customer }])
                .WithEligibleMenuItems([cappuccino])
                .Build();

            var excludedProduct = ProductBuilder
                .Simple()
                .WithName("Excluded Product")
                .WithDescription("Visible but not eligible for customer")
                .WithVisible(true)
                .WithProductUserGroup([new ProductUserGroup { UserGroup = UserGroup.Board }])
                .WithEligibleMenuItems([latte])
                .Build();

            InitialContext.Users.Add(user);
            InitialContext.Products.AddRange(hiddenOwnedProduct, availableProduct, excludedProduct);
            await InitialContext.SaveChangesAsync();

            var purchase1 = new Purchase
            {
                Product = hiddenOwnedProduct,
                ProductId = hiddenOwnedProduct.Id,
                ProductName = hiddenOwnedProduct.Name,
                Price = hiddenOwnedProduct.Price,
                NumberOfTickets = hiddenOwnedProduct.NumberOfTickets,
                OrderId = Guid.NewGuid().ToString(),
                Status = Models.DataTransferObjects.v2.Purchase.PurchaseStatus.Completed,
                Type = PurchaseType.Free,
                PurchasedBy = user,
                PurchasedById = user.Id,
                Tickets = [],
            };

            var purchase2 = new Purchase
            {
                Product = hiddenOwnedProduct,
                ProductId = hiddenOwnedProduct.Id,
                ProductName = hiddenOwnedProduct.Name,
                Price = hiddenOwnedProduct.Price,
                NumberOfTickets = hiddenOwnedProduct.NumberOfTickets,
                OrderId = Guid.NewGuid().ToString(),
                Status = Models.DataTransferObjects.v2.Purchase.PurchaseStatus.Completed,
                Type = PurchaseType.Free,
                PurchasedBy = user,
                PurchasedById = user.Id,
                Tickets = [],
            };

            var purchase3 = new Purchase
            {
                Product = hiddenOwnedProduct,
                ProductId = hiddenOwnedProduct.Id,
                ProductName = hiddenOwnedProduct.Name,
                Price = hiddenOwnedProduct.Price,
                NumberOfTickets = hiddenOwnedProduct.NumberOfTickets,
                OrderId = Guid.NewGuid().ToString(),
                Status = Models.DataTransferObjects.v2.Purchase.PurchaseStatus.Completed,
                Type = PurchaseType.Free,
                PurchasedBy = user,
                PurchasedById = user.Id,
                Tickets = [],
            };

            InitialContext.Tickets.AddRange(
                new Ticket
                {
                    Owner = user,
                    OwnerId = user.Id,
                    Purchase = purchase1,
                    ProductId = hiddenOwnedProduct.Id,
                    Status = TicketStatus.Unused,
                },
                new Ticket
                {
                    Owner = user,
                    OwnerId = user.Id,
                    Purchase = purchase2,
                    ProductId = hiddenOwnedProduct.Id,
                    Status = TicketStatus.Unused,
                },
                new Ticket
                {
                    Owner = user,
                    OwnerId = user.Id,
                    Purchase = purchase3,
                    ProductId = hiddenOwnedProduct.Id,
                    Status = TicketStatus.Used,
                }
            );

            await InitialContext.SaveChangesAsync();

            var assertionUser = AssertionContext.Users.First(u => u.Id == user.Id);

            using var service = new TicketService(
                AssertionContext,
                Mock.Of<Library.Services.v2.IStatisticService>(),
                Microsoft.Extensions.Logging.Abstractions.NullLogger<TicketService>.Instance
            );

            // Act
            var result = (await service.GetGroupedTicketsAsync(assertionUser)).ToList();

            // Assert
            Assert.Single(result);

            var ownedCard = Assert.Single(result.Where(r => r.ProductId == hiddenOwnedProduct.Id));
            Assert.Equal(hiddenOwnedProduct.Name, ownedCard.ProductName);
            Assert.Equal(2, ownedCard.TicketsLeft);
            Assert.Equal(2, ownedCard.EligibleMenuItems.Count());
            Assert.Contains(
                ownedCard.EligibleMenuItems,
                mi => mi.Name == "Cappuccino" && mi.Active
            );
            Assert.Contains(ownedCard.EligibleMenuItems, mi => mi.Name == "Latte" && !mi.Active);

            Assert.DoesNotContain(result, r => r.ProductId == availableProduct.Id);
            Assert.DoesNotContain(result, r => r.ProductId == excludedProduct.Id);
        }
    }
}
