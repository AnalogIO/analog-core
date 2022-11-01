using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services;
using CoffeeCard.MobilePay.Service.v2;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using ITicketService = CoffeeCard.Library.Services.v2.ITicketService;
using PurchaseService = CoffeeCard.Library.Services.v2.PurchaseService;

namespace CoffeeCard.Tests.Unit.Services.v2
{
    public class PurchaseServiceTests
    {
        [Theory(DisplayName = "UserMayIssueFreeProduct throws error given bad ids")]
        [InlineData(2,1)] // Bad UserId
        [InlineData(1,2)] // Bad ProductId
        [InlineData(1,3)] // Not matching UserGroups
        public async Task UserMayIssueFreeProductThrowsApiErrorGivenUnknownId(int userId, int productId)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(UserMayIssueFreeProductThrowsApiErrorGivenUnknownId) +
                                     userId + productId);

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            var user = new User
            {
                Id = 1,
                Name = "User1",
                Email = "email@email.test",
                Password = "password",
                Salt = "salt",
                DateCreated = new DateTime(year: 2020, month: 11, day: 11),
                IsVerified = true,
                PrivacyActivated = false,
                UserGroup = UserGroup.Customer,
                UserState = UserState.Active
            };
            context.Add(user);

            var pug = new ProductUserGroup
            {
                ProductId = 3, UserGroup = UserGroup.Barista
            };

            var product = new Product();
            context.Add(product);
            var product2 = new Product
            {
                Id = 3,
                Name = "Test3",
            };
            product2.ProductUserGroup = new List<ProductUserGroup> { pug };
            context.Add(product2);
            
            await context.SaveChangesAsync();

            var mobilePayService = new Mock<IMobilePayPaymentsService>();
            var ticketService = new Mock<ITicketService>();
            var mailService = new Mock<IEmailService>();
            var purchaseService = new PurchaseService(context, mobilePayService.Object, ticketService.Object, mailService.Object);

            var issueProductDto = new IssueProductDto
            {
                ProductId = productId,
                UserId = userId,
                IssuedBy = "Barista perk - App"
            };
            // Act
            var exception = await Assert.ThrowsAsync<ApiException>(() => purchaseService.IssueFreeProduct(issueProductDto)) ;
        }

        [Theory(DisplayName = "IssueFreeProduct adds tickets and purchase to user")]
        [MemberData(nameof(ProductGenerator))]
        public async Task IssueFreeProductAddsTicketsAndPurchaseToUser(Product product)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(UserMayIssueFreeProductThrowsApiErrorGivenUnknownId) +
                                     product.Name);

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            var user = new User
            {
                Id = 1,
                Name = "User1",
                Email = "email@email.test",
                Password = "password",
                Salt = "salt",
                DateCreated = new DateTime(year: 2020, month: 11, day: 11),
                IsVerified = true,
                PrivacyActivated = false,
                UserGroup = UserGroup.Customer,
                UserState = UserState.Active
            };
            context.Add(user);
            context.Add(product);
            await context.SaveChangesAsync();

            var mobilePayService = new Mock<IMobilePayPaymentsService>();
            var ticketService = new Mock<ITicketService>();
            var mailService = new Mock<IEmailService>();
            var purchaseService = new PurchaseService(context, mobilePayService.Object, ticketService.Object,
                mailService.Object);

            var issueProductDto = new IssueProductDto
            {
                ProductId = product.Id,
                UserId = user.Id,
                IssuedBy = "Barista perk - App"
            };

            // Act
            var purchaseResponse = await purchaseService.IssueFreeProduct(issueProductDto);
            
            // Assert
            var userUpdated = await context.Users.FindAsync(user.Id);
            
            Assert.True(userUpdated.Purchases.Count == 1);
            Assert.True(userUpdated.Tickets.Count == product.NumberOfTickets);
        }
        
        public static IEnumerable<object[]> ProductGenerator()
        {
            var pug = new List<ProductUserGroup> { new ProductUserGroup { ProductId = 1 } };
            yield return new object[] { 
                new Product{
                    Name = "Test1", 
                    Description = "Test1", 
                    Id = 1, 
                    NumberOfTickets = 1, 
                    ProductUserGroup = pug
                }
            };
            yield return new object[] { 
                new Product{
                    Name = "Test2", 
                    Description = "Test2", 
                    Id = 1, 
                    NumberOfTickets = 5, 
                    ProductUserGroup = pug
                }
            };
            yield return new object[] { 
                new Product{
                    Name = "Test3", 
                    Description = "Test3", 
                    Id = 1, 
                    NumberOfTickets = 10, 
                    ProductUserGroup = pug
                }
            };
        }
    }
}