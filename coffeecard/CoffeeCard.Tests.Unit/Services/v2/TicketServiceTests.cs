using System;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Models.Entities;
using Moq;
using Xunit;
using TicketService = CoffeeCard.Library.Services.v2.TicketService;

namespace CoffeeCard.Tests.Unit.Services.v2
{
    public class TicketServiceTests : BaseUnitTests
    {
        [Fact(DisplayName = "GetCoffeeCardsAsync returns owned products and includes menu items")]
        public async Task GetCoffeeCardsAsync_ReturnsCoffeeCardsWithMenuItems()
        {
            // Arrange
            var user = new User
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "password",
                Salt = "salt",
                IsVerified = true,
                Experience = 0,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                PrivacyActivated = true,
                Programme = new Programme { ShortName = "CS", FullName = "Computer Science" },
                Purchases = [],
                Statistics = [],
                Tokens = [],
                UserState = UserState.Active,
                UserGroup = UserGroup.Customer,
                Tickets = [],
            };

            var cappuccino = new MenuItem
            {
                Name = "Cappuccino",
                Active = true,
                AssociatedProducts = [],
                MenuItemProducts = [],
            };

            var latte = new MenuItem
            {
                Name = "Latte",
                Active = false,
                AssociatedProducts = [],
                MenuItemProducts = [],
            };

            var hiddenOwnedProduct = new Product
            {
                Name = "Owned Hidden Clip Card",
                Description = "Hidden product with tickets",
                NumberOfTickets = 10,
                Price = 100,
                ExperienceWorth = 0,
                Visible = false,
                ProductUserGroup = [new ProductUserGroup { UserGroup = UserGroup.Manager }],
                EligibleMenuItems = [cappuccino, latte],
                MenuItemProducts = [],
            };

            var availableProduct = new Product
            {
                Name = "Available Product",
                Description = "Visible and eligible for customer",
                NumberOfTickets = 8,
                Price = 75,
                ExperienceWorth = 0,
                Visible = true,
                ProductUserGroup = [new ProductUserGroup { UserGroup = UserGroup.Customer }],
                EligibleMenuItems = [cappuccino],
                MenuItemProducts = [],
            };

            var excludedProduct = new Product
            {
                Name = "Excluded Product",
                Description = "Visible but not eligible for customer",
                NumberOfTickets = 5,
                Price = 40,
                ExperienceWorth = 0,
                Visible = true,
                ProductUserGroup = [new ProductUserGroup { UserGroup = UserGroup.Board }],
                EligibleMenuItems = [latte],
                MenuItemProducts = [],
            };

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
            var result = (await service.GetCoffeeCardsAsync(assertionUser)).ToList();

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
