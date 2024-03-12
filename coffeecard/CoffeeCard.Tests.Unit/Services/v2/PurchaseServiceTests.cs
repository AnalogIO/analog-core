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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(
                    nameof(InitiatePurchaseCheckUserIsAllowedToPurchaseProductThrowsExceptionsInSeveralConditions) +
                    userId + productId);

            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            User user = new User
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
            _ = context.Add(user);

            Product product1 = new Product
            {
                Id = 1,
                Name = "Product1",
                Description = "desc",
                Price = 100
            };
            _ = context.Add(product1);

            ProductUserGroup pug1 = new ProductUserGroup
            {
                UserGroup = UserGroup.Customer,
                Product = product1
            };
            _ = context.Add(pug1);

            Product product2 = new Product
            {
                Id = 2,
                Name = "Product2",
                Description = "desc",
                Price = 100
            };
            _ = context.Add(product2);

            ProductUserGroup pug2 = new ProductUserGroup
            {
                UserGroup = UserGroup.Barista,
                Product = product2
            };
            _ = context.Add(pug2);

            _ = await context.SaveChangesAsync();

            Mock<IMobilePayPaymentsService> mobilePayService = new Mock<IMobilePayPaymentsService>();
            Mock<Library.Services.IEmailService> mailService = new Mock<Library.Services.IEmailService>();

            ProductService productService = new ProductService(context);
            TicketService ticketService = new TicketService(context, new Mock<IStatisticService>().Object);
            PurchaseService purchaseService = new PurchaseService(context, mobilePayService.Object, ticketService,
                mailService.Object, productService);

            InitiatePurchaseRequest request = new InitiatePurchaseRequest
            {
                PaymentType = PaymentType.FreePurchase,
                ProductId = productId
            };

            // Act, Assert
            _ = await Assert.ThrowsAsync(exceptionType, () => purchaseService.InitiatePurchase(request, user));
        }

        [Fact(DisplayName = "InitiatePurchase for PaymentType MobilePay")]
        public async Task InitiatePurchasePaymentTypeMobilePay()
        {
            // Arrange
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(InitiatePurchasePaymentTypeMobilePay));

            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            User user = new User
            {
                Id = 1,
                Name = "User1",
                Email = "test@test.test",
                Password = "pass",
                Salt = "salt",
                UserGroup = UserGroup.Customer,
            };
            _ = context.Add(user);

            Product product1 = new Product
            {
                Id = 1,
                Name = "Product1",
                Description = "desc",
                Price = 100
            };
            _ = context.Add(product1);

            ProductUserGroup pug1 = new ProductUserGroup
            {
                UserGroup = UserGroup.Customer,
                Product = product1
            };
            _ = context.Add(pug1);

            _ = await context.SaveChangesAsync();

            Mock<IMobilePayPaymentsService> mobilePayService = new Mock<IMobilePayPaymentsService>();
            Mock<Library.Services.IEmailService> mailService = new Mock<Library.Services.IEmailService>();

            ProductService productService = new ProductService(context);
            TicketService ticketService = new TicketService(context, new Mock<IStatisticService>().Object);
            PurchaseService purchaseService = new PurchaseService(context, mobilePayService.Object, ticketService,
                mailService.Object, productService);

            InitiatePurchaseRequest request = new InitiatePurchaseRequest
            {
                PaymentType = PaymentType.MobilePay,
                ProductId = product1.Id
            };

            string mobilepayPaymentId = Guid.NewGuid().ToString();
            string orderId = Guid.NewGuid().ToString();
            string mpDeepLink = "mobilepay://merchant_payments?payment_id=186d2b31-ff25-4414-9fd1-bfe9807fa8b7";
            _ = mobilePayService.Setup(mps => mps.InitiatePayment(It.IsAny<MobilePayPaymentRequest>()))
                .ReturnsAsync(new MobilePayPaymentDetails(orderId, mpDeepLink, mobilepayPaymentId));

            // Act
            InitiatePurchaseResponse result = await purchaseService.InitiatePurchase(request, user);
            Purchase purchaseInDatabase = await context.Purchases.FirstAsync();

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
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(InitiatePurchaseAddsTicketsToUserWhenFree) +
                                     product.Name);

            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            User user = new User
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
            _ = context.Add(user);
            _ = context.Add(product);
            _ = await context.SaveChangesAsync();

            Mock<IMobilePayPaymentsService> mobilePayService = new Mock<IMobilePayPaymentsService>();
            Mock<Library.Services.IEmailService> mailService = new Mock<Library.Services.IEmailService>();
            ProductService productService = new ProductService(context);
            TicketService ticketService = new TicketService(context, new Mock<IStatisticService>().Object);
            PurchaseService purchaseService = new PurchaseService(context, mobilePayService.Object, ticketService,
                mailService.Object, productService);

            InitiatePurchaseRequest request = new InitiatePurchaseRequest
            {
                PaymentType = PaymentType.FreePurchase,
                ProductId = product.Id
            };

            // Act
            InitiatePurchaseResponse purchaseResponse = await purchaseService.InitiatePurchase(request, user);

            // Assert
            User userUpdated = await context.Users.FindAsync(user.Id);

            Assert.Equal(1, userUpdated.Purchases.Count);
            Assert.Equal(product.NumberOfTickets, userUpdated.Tickets.Count);
        }

        public static IEnumerable<object[]> ProductGenerator()
        {
            List<ProductUserGroup> pug = [new ProductUserGroup { ProductId = 1 }];
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