using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.MobilePay.Generated.Api.ePaymentApi;
using CoffeeCard.MobilePay.Service.v2;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using PurchaseService = CoffeeCard.Library.Services.v2.PurchaseService;

namespace CoffeeCard.Tests.Unit.Services.v2
{
    public class PurchaseServiceTests
    {
        [Theory(
            DisplayName = "InitiatePurchase.CheckUserIsAllowedToPurchaseProduct throws exceptions in several conditions"
        )]
        [InlineData(1, 1, typeof(BadRequestException))] // FreePurchase PaymentType fails when product has a price != 0
        [InlineData(1, 2, typeof(IllegalUserOperationException))] // Product not in PUG
        [InlineData(1, 3, typeof(EntityNotFoundException))] // Product not exists
        public async Task InitiatePurchaseCheckUserIsAllowedToPurchaseProductThrowsExceptionsInSeveralConditions(
            int userId,
            int productId,
            Type exceptionType
        )
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(
                    InitiatePurchaseCheckUserIsAllowedToPurchaseProductThrowsExceptionsInSeveralConditions
                )
                    + userId
                    + productId
            );

            var databaseSettings = new DatabaseSettings { SchemaName = "test" };
            var environmentSettings = new EnvironmentSettings
            {
                EnvironmentType = EnvironmentType.Test,
            };

            await using var context = new CoffeeCardContext(
                builder.Options,
                databaseSettings,
                environmentSettings
            );
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
                UserState = UserState.Active,
            };
            context.Add(user);

            var product1 = new Product
            {
                Id = 1,
                Name = "Product1",
                Description = "desc",
                Price = 100,
            };
            context.Add(product1);

            var pug1 = new ProductUserGroup { UserGroup = UserGroup.Customer, Product = product1 };
            context.Add(pug1);

            var product2 = new Product
            {
                Id = 2,
                Name = "Product2",
                Description = "desc",
                Price = 100,
            };
            context.Add(product2);

            var pug2 = new ProductUserGroup { UserGroup = UserGroup.Barista, Product = product2 };
            context.Add(pug2);

            await context.SaveChangesAsync();

            var mobilePayService = new Mock<IMobilePayPaymentsService>();
            var mailService = new Mock<Library.Services.IEmailService>();

            var productService = new ProductService(context, NullLogger<ProductService>.Instance);
            var ticketService = new TicketService(
                context,
                new Mock<IStatisticService>().Object,
                NullLogger<TicketService>.Instance
            );
            var purchaseService = new PurchaseService(
                context,
                mobilePayService.Object,
                ticketService,
                mailService.Object,
                productService,
                NullLogger<PurchaseService>.Instance
            );

            var request = new InitiatePurchaseRequest
            {
                PaymentType = PaymentType.FreePurchase,
                ProductId = productId,
            };

            // Act, Assert
            await Assert.ThrowsAsync(
                exceptionType,
                () => purchaseService.InitiatePurchase(request, user)
            );
        }

        [Fact(DisplayName = "InitiatePurchase for PaymentType MobilePay")]
        public async Task InitiatePurchasePaymentTypeMobilePay()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(InitiatePurchasePaymentTypeMobilePay)
            );

            var databaseSettings = new DatabaseSettings { SchemaName = "test" };
            var environmentSettings = new EnvironmentSettings
            {
                EnvironmentType = EnvironmentType.Test,
            };

            await using var context = new CoffeeCardContext(
                builder.Options,
                databaseSettings,
                environmentSettings
            );
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
                Price = 100,
            };
            context.Add(product1);

            var pug1 = new ProductUserGroup { UserGroup = UserGroup.Customer, Product = product1 };
            context.Add(pug1);

            await context.SaveChangesAsync();

            var mobilePayService = new Mock<IMobilePayPaymentsService>();
            var mailService = new Mock<Library.Services.IEmailService>();

            var productService = new ProductService(context, NullLogger<ProductService>.Instance);
            var ticketService = new TicketService(
                context,
                new Mock<IStatisticService>().Object,
                NullLogger<TicketService>.Instance
            );
            var purchaseService = new PurchaseService(
                context,
                mobilePayService.Object,
                ticketService,
                mailService.Object,
                productService,
                NullLogger<PurchaseService>.Instance
            );

            var request = new InitiatePurchaseRequest
            {
                PaymentType = PaymentType.MobilePay,
                ProductId = product1.Id,
            };

            var mobilepayPaymentId = Guid.Parse("186d2b31-ff25-4414-9fd1-bfe9807fa8b7").ToString();
            const string mpDeepLink =
                "mobilepay://merchant_payments?payment_id=186d2b31-ff25-4414-9fd1-bfe9807fa8b7";
            mobilePayService
                .Setup(mps => mps.InitiatePayment(It.IsAny<MobilePayPaymentRequest>()))
                .ReturnsAsync(
                    new MobilePayPaymentDetails
                    {
                        PaymentId = mobilepayPaymentId,
                        MobilePayAppRedirectUri = mpDeepLink,
                    }
                );

            // Act
            var result = await purchaseService.InitiatePurchase(request, user);
            var purchaseInDatabase = await context
                .Purchases.Include(purchase => purchase.PurchasedBy)
                .FirstAsync();

            // Assert
            // DB Contents
            Assert.Equal(mobilepayPaymentId, purchaseInDatabase.ExternalTransactionId);

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
            Assert.Equal(
                mobilepayPaymentId,
                ((result.PaymentDetails as MobilePayPaymentDetails)!).PaymentId
            );
            Assert.Equal(
                mpDeepLink,
                ((result.PaymentDetails as MobilePayPaymentDetails)!).MobilePayAppRedirectUri
            );
        }

        [Theory(DisplayName = "InitiatePurchase adds tickets to user when free")]
        [MemberData(nameof(ProductGenerator))]
        public async Task InitiatePurchaseAddsTicketsToUserWhenFree(Product product)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(InitiatePurchaseAddsTicketsToUserWhenFree) + product.Name
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
                UserState = UserState.Active,
            };
            context.Add(user);
            context.Add(product);
            await context.SaveChangesAsync();

            var mobilePayService = new Mock<IMobilePayPaymentsService>();
            var mailService = new Mock<Library.Services.IEmailService>();
            var productService = new ProductService(context, NullLogger<ProductService>.Instance);
            var ticketService = new TicketService(
                context,
                new Mock<IStatisticService>().Object,
                NullLogger<TicketService>.Instance
            );
            var purchaseService = new PurchaseService(
                context,
                mobilePayService.Object,
                ticketService,
                mailService.Object,
                productService,
                NullLogger<PurchaseService>.Instance
            );

            var request = new InitiatePurchaseRequest
            {
                PaymentType = PaymentType.FreePurchase,
                ProductId = product.Id,
            };

            // Act
            var purchaseResponse = await purchaseService.InitiatePurchase(request, user);

            // Assert
            var userUpdated = await context.Users.FindAsync(user.Id);

            Assert.Single(userUpdated!.Purchases);
            Assert.Equal(product.NumberOfTickets, userUpdated.Tickets.Count);
        }

        [Theory(DisplayName = "RefundPurchase refunds a purchase")]
        [MemberData(nameof(ProductGenerator))]
        public async Task RefundPurchaseRefundsAPurchase(Product product)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(RefundPurchaseRefundsAPurchase) + product.Name
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
                UserGroup = UserGroup.Board,
                UserState = UserState.Active,
            };

            var purchase = new Purchase
            {
                Id = 1,
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                NumberOfTickets = product.NumberOfTickets,
                ExternalTransactionId = Guid.NewGuid().ToString(),
                PurchasedBy = user,
                OrderId = "test",
                Tickets = new List<Ticket>
                {
                    new Ticket
                    {
                        Id = 1,
                        ProductId = product.Id,
                        Status = TicketStatus.Unused,
                        Owner = user,
                    },
                },
            };
            context.Add(user);
            context.Add(product);
            context.Add(purchase);
            await context.SaveChangesAsync();

            var mobilePayService = new Mock<IMobilePayPaymentsService>();
            mobilePayService
                .Setup(mps => mps.RefundPayment(It.IsAny<Purchase>(), It.IsAny<int>()))
                .ReturnsAsync(true);
            var mailService = new Mock<Library.Services.IEmailService>();
            var productService = new ProductService(context, NullLogger<ProductService>.Instance);
            var ticketService = new TicketService(
                context,
                new Mock<IStatisticService>().Object,
                NullLogger<TicketService>.Instance
            );
            var purchaseService = new PurchaseService(
                context,
                mobilePayService.Object,
                ticketService,
                mailService.Object,
                productService,
                NullLogger<PurchaseService>.Instance
            );

            // Act
            var refund = await purchaseService.RefundPurchase(purchase.Id);

            // Assert
            Assert.Equal(PurchaseStatus.Refunded, refund.PurchaseStatus);
        }

        [Theory(DisplayName = "RefundPurchase throws exception when purchase not found")]
        [MemberData(nameof(ProductGenerator))]
        public async Task RefundPurchaseThrowsExceptionWhenNotAllowed(Product product)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(RefundPurchaseThrowsExceptionWhenNotAllowed) + product.Name
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
                UserGroup = UserGroup.Board,
                UserState = UserState.Active,
            };

            var purchase = new Purchase
            {
                Id = 1,
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                NumberOfTickets = product.NumberOfTickets,
                ExternalTransactionId = Guid.NewGuid().ToString(),
                PurchasedBy = user,
                OrderId = "test",
            };
            context.Add(user);
            context.Add(product);
            context.Add(purchase);
            await context.SaveChangesAsync();

            var mobilePayService = new Mock<IMobilePayPaymentsService>();
            mobilePayService
                .Setup(mps => mps.RefundPayment(It.IsAny<Purchase>(), It.IsAny<int>()))
                .ReturnsAsync(true);
            var mailService = new Mock<Library.Services.IEmailService>();
            var productService = new ProductService(context, NullLogger<ProductService>.Instance);
            var ticketService = new TicketService(
                context,
                new Mock<IStatisticService>().Object,
                NullLogger<TicketService>.Instance
            );
            var purchaseService = new PurchaseService(
                context,
                mobilePayService.Object,
                ticketService,
                mailService.Object,
                productService,
                NullLogger<PurchaseService>.Instance
            );

            // Act, Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                purchaseService.RefundPurchase(2)
            );
        }

        [Theory(DisplayName = "RefundPurchase throws exception when purchase is already refunded")]
        [MemberData(nameof(ProductGenerator))]
        public async Task RefundPurchaseThrowsExceptionWhenAlreadyRefunded(Product product)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(RefundPurchaseThrowsExceptionWhenAlreadyRefunded) + product.Name
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
                UserGroup = UserGroup.Board,
                UserState = UserState.Active,
            };

            var purchase = new Purchase
            {
                Id = 1,
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                NumberOfTickets = product.NumberOfTickets,
                ExternalTransactionId = Guid.NewGuid().ToString(),
                PurchasedBy = user,
                OrderId = "test",
                Status = PurchaseStatus.Refunded,
            };
            context.Add(user);
            context.Add(product);
            context.Add(purchase);
            await context.SaveChangesAsync();

            var mobilePayService = new Mock<IMobilePayPaymentsService>();
            mobilePayService
                .Setup(mps => mps.RefundPayment(It.IsAny<Purchase>(), It.IsAny<int>()))
                .ReturnsAsync(true);
            var mailService = new Mock<Library.Services.IEmailService>();
            var productService = new ProductService(context, NullLogger<ProductService>.Instance);
            var ticketService = new TicketService(
                context,
                new Mock<IStatisticService>().Object,
                NullLogger<TicketService>.Instance
            );
            var purchaseService = new PurchaseService(
                context,
                mobilePayService.Object,
                ticketService,
                mailService.Object,
                productService,
                NullLogger<PurchaseService>.Instance
            );

            // Act, Assert
            await Assert.ThrowsAsync<IllegalUserOperationException>(() =>
                purchaseService.RefundPurchase(product.Id)
            );
        }

        [Theory(DisplayName = "GetPurchases returns all purchases for a user")]
        [MemberData(nameof(ProductGenerator))]
        public async Task GetPurchasesReturnsAllPurchasesForAUser(Product product)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(GetPurchasesReturnsAllPurchasesForAUser) + product.Name
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
                UserGroup = UserGroup.Board,
                UserState = UserState.Active,
            };

            var purchase = new Purchase
            {
                Id = 1,
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                NumberOfTickets = product.NumberOfTickets,
                ExternalTransactionId = Guid.NewGuid().ToString(),
                PurchasedBy = user,
                OrderId = "test",
                Status = PurchaseStatus.Refunded,
            };
            context.Add(user);
            context.Add(product);
            context.Add(purchase);
            await context.SaveChangesAsync();

            var mobilePayService = new Mock<IMobilePayPaymentsService>();
            var mailService = new Mock<Library.Services.IEmailService>();
            var productService = new ProductService(context, NullLogger<ProductService>.Instance);
            var ticketService = new TicketService(
                context,
                new Mock<IStatisticService>().Object,
                NullLogger<TicketService>.Instance
            );
            var purchaseService = new PurchaseService(
                context,
                mobilePayService.Object,
                ticketService,
                mailService.Object,
                productService,
                NullLogger<PurchaseService>.Instance
            );

            // act
            var result = await purchaseService.GetPurchases(user);

            // Assert
            Assert.Single(result);
        }

        [Theory(DisplayName = "GetPurchases given a faulty id throws exception")]
        [MemberData(nameof(ProductGenerator))]
        public async Task GetPurchasesGivenAFaultyIdThrowsNotFoundException(Product product)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(GetPurchasesGivenAFaultyIdThrowsNotFoundException) + product.Name
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
                UserGroup = UserGroup.Board,
                UserState = UserState.Active,
            };

            var purchase = new Purchase
            {
                Id = 1,
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                NumberOfTickets = product.NumberOfTickets,
                ExternalTransactionId = Guid.NewGuid().ToString(),
                PurchasedBy = user,
                OrderId = "test",
                Status = PurchaseStatus.Refunded,
            };
            context.Add(user);
            context.Add(product);
            context.Add(purchase);
            await context.SaveChangesAsync();

            var mobilePayService = new Mock<IMobilePayPaymentsService>();
            var mailService = new Mock<Library.Services.IEmailService>();
            var productService = new ProductService(context, NullLogger<ProductService>.Instance);
            var ticketService = new TicketService(
                context,
                new Mock<IStatisticService>().Object,
                NullLogger<TicketService>.Instance
            );
            var purchaseService = new PurchaseService(
                context,
                mobilePayService.Object,
                ticketService,
                mailService.Object,
                productService,
                NullLogger<PurchaseService>.Instance
            );

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                purchaseService.GetPurchases(10)
            );
        }

        [Theory(
            DisplayName = "HandleMobilePayPaymentUpdate completes purchase on AUTHORIZED event"
        )]
        [MemberData(nameof(ProductGenerator))]
        public async Task HandleMobilePayPaymentUpdate_Authorized_CompletesPurchase(Product product)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(HandleMobilePayPaymentUpdate_Authorized_CompletesPurchase) + product.Name
            );

            var databaseSettings = new DatabaseSettings { SchemaName = "test" };
            var environmentSettings = new EnvironmentSettings
            {
                EnvironmentType = EnvironmentType.Test,
            };

            await using var context = new CoffeeCardContext(
                builder.Options,
                databaseSettings,
                environmentSettings
            );

            var transactionId = Guid.NewGuid().ToString();
            var user = new User
            {
                Id = 1,
                Name = "User1",
                Email = "email@email.test",
                Password = "password",
                Salt = "salt",
                UserGroup = UserGroup.Customer,
                UserState = UserState.Active,
            };

            var purchase = new Purchase
            {
                Id = 1,
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                NumberOfTickets = product.NumberOfTickets,
                ExternalTransactionId = transactionId,
                PurchasedBy = user,
                OrderId = "test-order-id",
                Status = PurchaseStatus.PendingPayment,
            };

            context.Add(user);
            context.Add(product);
            context.Add(purchase);
            await context.SaveChangesAsync();

            var mobilePayService = new Mock<IMobilePayPaymentsService>();
            mobilePayService
                .Setup(mps => mps.CapturePayment(Guid.Parse(transactionId), product.Price))
                .Returns(Task.CompletedTask);

            var mailService = new Mock<Library.Services.IEmailService>();
            mailService
                .Setup(m => m.SendInvoiceAsyncV2(It.IsAny<Purchase>(), It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            var productService = new ProductService(context, NullLogger<ProductService>.Instance);
            var ticketService = new TicketService(
                context,
                new Mock<IStatisticService>().Object,
                NullLogger<TicketService>.Instance
            );
            var purchaseService = new PurchaseService(
                context,
                mobilePayService.Object,
                ticketService,
                mailService.Object,
                productService,
                NullLogger<PurchaseService>.Instance
            );

            var webhookEvent = new WebhookEvent
            {
                Name = PaymentEventName.AUTHORIZED,
                Reference = transactionId,
            };

            // Act
            await purchaseService.HandleMobilePayPaymentUpdate(webhookEvent);

            // Assert
            var updatedPurchase = await context.Purchases.FindAsync(purchase.Id);
            Assert.Equal(PurchaseStatus.Completed, updatedPurchase.Status);

            mobilePayService.Verify(
                m => m.CapturePayment(Guid.Parse(transactionId), product.Price),
                Times.Once
            );
            mailService.Verify(
                m => m.SendInvoiceAsyncV2(It.IsAny<Purchase>(), It.IsAny<User>()),
                Times.Once
            );
        }

        [Theory(DisplayName = "HandleMobilePayPaymentUpdate cancels purchase on CANCELLED event")]
        [MemberData(nameof(ProductGenerator))]
        public async Task HandleMobilePayPaymentUpdate_Cancelled_CancelsPurchase(Product product)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(HandleMobilePayPaymentUpdate_Cancelled_CancelsPurchase) + product.Name
            );

            var databaseSettings = new DatabaseSettings { SchemaName = "test" };
            var environmentSettings = new EnvironmentSettings
            {
                EnvironmentType = EnvironmentType.Test,
            };

            await using var context = new CoffeeCardContext(
                builder.Options,
                databaseSettings,
                environmentSettings
            );

            var transactionId = Guid.NewGuid().ToString();
            var user = new User
            {
                Id = 1,
                Name = "User1",
                Email = "email@email.test",
                Password = "password",
                Salt = "salt",
                UserGroup = UserGroup.Customer,
                UserState = UserState.Active,
            };

            var purchase = new Purchase
            {
                Id = 1,
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                NumberOfTickets = product.NumberOfTickets,
                ExternalTransactionId = transactionId,
                PurchasedBy = user,
                OrderId = "test-order-id",
                Status = PurchaseStatus.PendingPayment,
            };

            context.Add(user);
            context.Add(product);
            context.Add(purchase);
            await context.SaveChangesAsync();

            var mobilePayService = new Mock<IMobilePayPaymentsService>();
            mobilePayService
                .Setup(mps => mps.CancelPayment(Guid.Parse(transactionId)))
                .Returns(Task.CompletedTask);

            var mailService = new Mock<Library.Services.IEmailService>();
            var productService = new ProductService(context, NullLogger<ProductService>.Instance);
            var ticketService = new TicketService(
                context,
                new Mock<IStatisticService>().Object,
                NullLogger<TicketService>.Instance
            );
            var purchaseService = new PurchaseService(
                context,
                mobilePayService.Object,
                ticketService,
                mailService.Object,
                productService,
                NullLogger<PurchaseService>.Instance
            );

            var webhookEvent = new WebhookEvent
            {
                Name = PaymentEventName.CANCELLED,
                Reference = transactionId,
            };

            // Act
            await purchaseService.HandleMobilePayPaymentUpdate(webhookEvent);

            // Assert
            var updatedPurchase = await context.Purchases.FindAsync(purchase.Id);
            Assert.Equal(PurchaseStatus.Cancelled, updatedPurchase.Status);

            mobilePayService.Verify(m => m.CancelPayment(Guid.Parse(transactionId)), Times.Once);
        }

        [Theory(DisplayName = "HandleMobilePayPaymentUpdate aborts purchase on ABORTED event")]
        [MemberData(nameof(ProductGenerator))]
        public async Task HandleMobilePayPaymentUpdate_Aborted_AbortsPurchase(Product product)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(HandleMobilePayPaymentUpdate_Aborted_AbortsPurchase) + product.Name
            );

            var databaseSettings = new DatabaseSettings { SchemaName = "test" };
            var environmentSettings = new EnvironmentSettings
            {
                EnvironmentType = EnvironmentType.Test,
            };

            await using var context = new CoffeeCardContext(
                builder.Options,
                databaseSettings,
                environmentSettings
            );

            var transactionId = Guid.NewGuid().ToString();
            var user = new User
            {
                Id = 1,
                Name = "User1",
                Email = "email@email.test",
                Password = "password",
                Salt = "salt",
                UserGroup = UserGroup.Customer,
                UserState = UserState.Active,
            };

            var purchase = new Purchase
            {
                Id = 1,
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                NumberOfTickets = product.NumberOfTickets,
                ExternalTransactionId = transactionId,
                PurchasedBy = user,
                OrderId = "test-order-id",
                Status = PurchaseStatus.PendingPayment,
            };

            context.Add(user);
            context.Add(product);
            context.Add(purchase);
            await context.SaveChangesAsync();

            var mobilePayService = new Mock<IMobilePayPaymentsService>();
            var mailService = new Mock<Library.Services.IEmailService>();
            var productService = new ProductService(context, NullLogger<ProductService>.Instance);
            var ticketService = new TicketService(
                context,
                new Mock<IStatisticService>().Object,
                NullLogger<TicketService>.Instance
            );
            var purchaseService = new PurchaseService(
                context,
                mobilePayService.Object,
                ticketService,
                mailService.Object,
                productService,
                NullLogger<PurchaseService>.Instance
            );

            var webhookEvent = new WebhookEvent
            {
                Name = PaymentEventName.ABORTED,
                Reference = transactionId,
            };

            // Act
            await purchaseService.HandleMobilePayPaymentUpdate(webhookEvent);

            // Assert
            var updatedPurchase = await context.Purchases.FindAsync(purchase.Id);
            Assert.Equal(PurchaseStatus.Cancelled, updatedPurchase.Status);
        }

        [Theory(DisplayName = "HandleMobilePayPaymentUpdate cancels purchase on EXPIRED event")]
        [MemberData(nameof(ProductGenerator))]
        public async Task HandleMobilePayPaymentUpdate_Expired_CancelsPurchase(Product product)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(HandleMobilePayPaymentUpdate_Expired_CancelsPurchase) + product.Name
            );

            var databaseSettings = new DatabaseSettings { SchemaName = "test" };
            var environmentSettings = new EnvironmentSettings
            {
                EnvironmentType = EnvironmentType.Test,
            };

            await using var context = new CoffeeCardContext(
                builder.Options,
                databaseSettings,
                environmentSettings
            );

            var transactionId = Guid.NewGuid().ToString();
            var user = new User
            {
                Id = 1,
                Name = "User1",
                Email = "email@email.test",
                Password = "password",
                Salt = "salt",
                UserGroup = UserGroup.Customer,
                UserState = UserState.Active,
            };

            var purchase = new Purchase
            {
                Id = 1,
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                NumberOfTickets = product.NumberOfTickets,
                ExternalTransactionId = transactionId,
                PurchasedBy = user,
                OrderId = "test-order-id",
                Status = PurchaseStatus.PendingPayment,
            };

            context.Add(user);
            context.Add(product);
            context.Add(purchase);
            await context.SaveChangesAsync();

            var mobilePayService = new Mock<IMobilePayPaymentsService>();
            mobilePayService
                .Setup(mps => mps.CancelPayment(Guid.Parse(transactionId)))
                .Returns(Task.CompletedTask);

            var mailService = new Mock<Library.Services.IEmailService>();
            var productService = new ProductService(context, NullLogger<ProductService>.Instance);
            var ticketService = new TicketService(
                context,
                new Mock<IStatisticService>().Object,
                NullLogger<TicketService>.Instance
            );
            var purchaseService = new PurchaseService(
                context,
                mobilePayService.Object,
                ticketService,
                mailService.Object,
                productService,
                NullLogger<PurchaseService>.Instance
            );

            var webhookEvent = new WebhookEvent
            {
                Name = PaymentEventName.EXPIRED,
                Reference = transactionId,
            };

            // Act
            await purchaseService.HandleMobilePayPaymentUpdate(webhookEvent);

            // Assert
            var updatedPurchase = await context.Purchases.FindAsync(purchase.Id);
            Assert.Equal(PurchaseStatus.Expired, updatedPurchase.Status);
        }

        [Fact(
            DisplayName = "HandleMobilePayPaymentUpdate throws exception when transaction not found"
        )]
        public async Task HandleMobilePayPaymentUpdate_TransactionNotFound_ThrowsException()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(HandleMobilePayPaymentUpdate_TransactionNotFound_ThrowsException)
            );

            var databaseSettings = new DatabaseSettings { SchemaName = "test" };
            var environmentSettings = new EnvironmentSettings
            {
                EnvironmentType = EnvironmentType.Test,
            };

            await using var context = new CoffeeCardContext(
                builder.Options,
                databaseSettings,
                environmentSettings
            );

            var mobilePayService = new Mock<IMobilePayPaymentsService>();
            var mailService = new Mock<Library.Services.IEmailService>();
            var productService = new ProductService(context, NullLogger<ProductService>.Instance);
            var ticketService = new TicketService(
                context,
                new Mock<IStatisticService>().Object,
                NullLogger<TicketService>.Instance
            );
            var purchaseService = new PurchaseService(
                context,
                mobilePayService.Object,
                ticketService,
                mailService.Object,
                productService,
                NullLogger<PurchaseService>.Instance
            );

            var webhookEvent = new WebhookEvent
            {
                Name = PaymentEventName.AUTHORIZED,
                Reference = Guid.NewGuid().ToString(),
            };

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                purchaseService.HandleMobilePayPaymentUpdate(webhookEvent)
            );
        }

        [Theory(
            DisplayName = "HandleMobilePayPaymentUpdate does nothing when purchase already completed"
        )]
        [MemberData(nameof(ProductGenerator))]
        public async Task HandleMobilePayPaymentUpdate_PurchaseAlreadyCompleted_DoesNothing(
            Product product
        )
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(HandleMobilePayPaymentUpdate_PurchaseAlreadyCompleted_DoesNothing)
                    + product.Name
            );

            var databaseSettings = new DatabaseSettings { SchemaName = "test" };
            var environmentSettings = new EnvironmentSettings
            {
                EnvironmentType = EnvironmentType.Test,
            };

            await using var context = new CoffeeCardContext(
                builder.Options,
                databaseSettings,
                environmentSettings
            );

            var transactionId = Guid.NewGuid().ToString();
            var user = new User
            {
                Id = 1,
                Name = "User1",
                Email = "email@email.test",
                Password = "password",
                Salt = "salt",
                UserGroup = UserGroup.Customer,
                UserState = UserState.Active,
            };

            var purchase = new Purchase
            {
                Id = 1,
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                NumberOfTickets = product.NumberOfTickets,
                ExternalTransactionId = transactionId,
                PurchasedBy = user,
                OrderId = "test-order-id",
                Status = PurchaseStatus.Completed,
            };

            context.Add(user);
            context.Add(product);
            context.Add(purchase);
            await context.SaveChangesAsync();

            var mobilePayService = new Mock<IMobilePayPaymentsService>();
            var mailService = new Mock<Library.Services.IEmailService>();
            var productService = new ProductService(context, NullLogger<ProductService>.Instance);
            var ticketService = new TicketService(
                context,
                new Mock<IStatisticService>().Object,
                NullLogger<TicketService>.Instance
            );
            var purchaseService = new PurchaseService(
                context,
                mobilePayService.Object,
                ticketService,
                mailService.Object,
                productService,
                NullLogger<PurchaseService>.Instance
            );

            var webhookEvent = new WebhookEvent
            {
                Name = PaymentEventName.AUTHORIZED,
                Reference = transactionId,
            };

            // Act
            await purchaseService.HandleMobilePayPaymentUpdate(webhookEvent);

            // Assert - The purchase status should still be Completed
            var updatedPurchase = await context.Purchases.FindAsync(purchase.Id);
            Assert.Equal(PurchaseStatus.Completed, updatedPurchase.Status);

            // Verify that no further processing was done
            mobilePayService.Verify(
                m => m.CapturePayment(It.IsAny<Guid>(), It.IsAny<int>()),
                Times.Never
            );
            mailService.Verify(
                m => m.SendInvoiceAsyncV2(It.IsAny<Purchase>(), It.IsAny<User>()),
                Times.Never
            );
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
                    ProductUserGroup = pug,
                },
            };
            yield return new object[]
            {
                new Product
                {
                    Name = "Test2",
                    Description = "Test2",
                    Id = 1,
                    NumberOfTickets = 5,
                    ProductUserGroup = pug,
                },
            };
            yield return new object[]
            {
                new Product
                {
                    Name = "Test3",
                    Description = "Test3",
                    Id = 1,
                    NumberOfTickets = 10,
                    ProductUserGroup = pug,
                },
            };
        }
    }
}
