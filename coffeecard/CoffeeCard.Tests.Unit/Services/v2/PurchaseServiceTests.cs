using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.MobilePay.Service.v2;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using PurchaseService = CoffeeCard.Library.Services.v2.PurchaseService;

namespace CoffeeCard.Tests.Unit.Services.v2
{
    public class PurchaseServiceTests
    {
        [Theory(DisplayName =
            "InitiatePurchase.CheckUserIsAllowedToPurchaseProduct throws exceptions in several conditions")]
        [InlineData(1, 1, typeof(ArgumentException))] // FreePurchase PaymentType fails when product has a price != 0
        [InlineData(1, 2, typeof(IllegalUserOperationException))] // Product not in PUG
        [InlineData(1, 3, typeof(EntityNotFoundException))] // Product not exists
        public async Task InitiatePurchaseCheckUserIsAllowedToPurchaseProductThrowsExceptionsInSeveralConditions(
            int userId, int productId, Type exceptionType)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(
                    nameof(InitiatePurchaseCheckUserIsAllowedToPurchaseProductThrowsExceptionsInSeveralConditions) +
                    userId + productId);

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings
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

            var product1 = new Product
            {
                Id = 1,
                Name = "Product1",
                Description = "desc",
                Price = 100
            };
            context.Add(product1);

            var pug1 = new ProductUserGroup
            {
                UserGroup = UserGroup.Customer,
                Product = product1
            };
            context.Add(pug1);

            var product2 = new Product
            {
                Id = 2,
                Name = "Product2",
                Description = "desc",
                Price = 100
            };
            context.Add(product2);

            var pug2 = new ProductUserGroup
            {
                UserGroup = UserGroup.Barista,
                Product = product2
            };
            context.Add(pug2);

            await context.SaveChangesAsync();

            var mobilePayService = new Mock<IMobilePayPaymentsService>();
            var mailService = new Mock<Library.Services.IEmailService>();

            var productService = new ProductService(context);
            var ticketService = new TicketService(context, new Mock<IStatisticService>().Object);
            var purchaseService = new PurchaseService(context, mobilePayService.Object, ticketService,
                mailService.Object, productService);

            var request = new InitiatePurchaseRequest
            {
                PaymentType = PaymentType.FreePurchase,
                ProductId = productId
            };

            // Act, Assert
            await Assert.ThrowsAsync(exceptionType, () => purchaseService.InitiatePurchase(request, user));
        }

        [Fact(DisplayName = "InitiatePurchase for PaymentType MobilePay")]
        public async Task InitiatePurchasePaymentTypeMobilePay()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(InitiatePurchasePaymentTypeMobilePay));

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            var user = new User
            {
                Id = 1,
                Name = "User1",
                Email = "test@test.test",
                Password = "pass",
                Salt = "salt",
                UserGroup = UserGroup.Customer,
            };
            context.Add(user);

            var product1 = new Product
            {
                Id = 1,
                Name = "Product1",
                Description = "desc",
                Price = 100
            };
            context.Add(product1);

            var pug1 = new ProductUserGroup
            {
                UserGroup = UserGroup.Customer,
                Product = product1
            };
            context.Add(pug1);

            await context.SaveChangesAsync();

            var mobilePayService = new Mock<IMobilePayPaymentsService>();
            var mailService = new Mock<Library.Services.IEmailService>();

            var productService = new ProductService(context);
            var ticketService = new TicketService(context, new Mock<IStatisticService>().Object);
            var purchaseService = new PurchaseService(context, mobilePayService.Object, ticketService,
                mailService.Object, productService);

            var request = new InitiatePurchaseRequest
            {
                PaymentType = PaymentType.MobilePay,
                ProductId = product1.Id
            };

            var mobilepayPaymentId = Guid.NewGuid().ToString();
            var orderId = Guid.NewGuid().ToString();
            var mpDeepLink = "mobilepay://merchant_payments?payment_id=186d2b31-ff25-4414-9fd1-bfe9807fa8b7";
            mobilePayService.Setup(mps => mps.InitiatePayment(It.IsAny<MobilePayPaymentRequest>()))
                .ReturnsAsync(new MobilePayPaymentDetails(orderId, mpDeepLink, mobilepayPaymentId, "Initiated"));

            // Act
            var result = await purchaseService.InitiatePurchase(request, user);
            var purchaseInDatabase = await context.Purchases.FirstAsync();

            // Assert
            // DB Contents
            Assert.Equal(mobilepayPaymentId, purchaseInDatabase.ExternalTransactionId);
            Assert.Equal(orderId, purchaseInDatabase.OrderId);

            Assert.Equal(product1.Id, purchaseInDatabase.ProductId);
            Assert.Equal(product1.Name, purchaseInDatabase.ProductName);
            Assert.Equal(product1.Price, purchaseInDatabase.Price);
            Assert.Equal(product1.NumberOfTickets, purchaseInDatabase.NumberOfTickets);
            Assert.Equal(user, purchaseInDatabase.PurchasedBy);

            // Result value
            Assert.Equal(product1.Id, result.Id);
            Assert.Equal(product1.Name, result.ProductName);
            Assert.Equal(product1.Price, result.TotalAmount);
            Assert.Equal(PurchaseStatus.PendingPayment, result.PurchaseStatus);

            Assert.Equal(PaymentType.MobilePay, result.PaymentDetails.PaymentType);
            Assert.Equal(orderId, result.PaymentDetails.OrderId);
            Assert.Equal(mobilepayPaymentId, (result.PaymentDetails as MobilePayPaymentDetails).PaymentId);
            Assert.Equal(mpDeepLink, (result.PaymentDetails as MobilePayPaymentDetails).MobilePayAppRedirectUri);
        }

        [Theory(DisplayName = "InitiatePurchase adds tickets to user when free")]
        [MemberData(nameof(ProductGenerator))]
        public async Task InitiatePurchaseAddsTicketsToUserWhenFree(Product product)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(InitiatePurchaseAddsTicketsToUserWhenFree) +
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
            var mailService = new Mock<Library.Services.IEmailService>();
            var productService = new ProductService(context);
            var ticketService = new TicketService(context, new Mock<IStatisticService>().Object);
            var purchaseService = new PurchaseService(context, mobilePayService.Object, ticketService,
                mailService.Object, productService);

            var request = new InitiatePurchaseRequest
            {
                PaymentType = PaymentType.FreePurchase,
                ProductId = product.Id
            };

            // Act
            var purchaseResponse = await purchaseService.InitiatePurchase(request, user);

            // Assert
            var userUpdated = await context.Users.FindAsync(user.Id);

            Assert.Equal(1, userUpdated.Purchases.Count);
            Assert.Equal(product.NumberOfTickets, userUpdated.Tickets.Count);
        }

        public static IEnumerable<object[]> ProductGenerator()
        {
            var pug = new List<ProductUserGroup> { new ProductUserGroup { ProductId = 1 } };
            yield return new object[]
            {
                new Product
                {
                    Name = "Test1",
                    Description = "Test1",
                    Id = 1,
                    NumberOfTickets = 1,
                    ProductUserGroup = pug
                }
            };
            yield return new object[]
            {
                new Product
                {
                    Name = "Test2",
                    Description = "Test2",
                    Id = 1,
                    NumberOfTickets = 5,
                    ProductUserGroup = pug
                }
            };
            yield return new object[]
            {
                new Product
                {
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