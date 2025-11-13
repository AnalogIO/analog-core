using System.Linq;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Models.DataTransferObjects.v2.Voucher;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services.v2
{
    public class VoucherServiceTests
    {
        [Theory(DisplayName = "CreateVouchers returns unique responses")]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(10, 1)]
        [InlineData(100, 1)]
        [InlineData(50000, 1)]
        public async void CreateVouchersReturnsUniqueResponses(int amount, int productId)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(CreateVouchersReturnsUniqueResponses) + amount + productId
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

            var product = new Product
            {
                Id = 1,
                Name = "product",
                Description = "desc",
            };
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();
            var voucherService = new VoucherService(context);

            // Act
            var request = new IssueVoucherRequest { Amount = amount, ProductId = productId };
            var responses = (await voucherService.CreateVouchers(request)).ToList();

            // Assert
            Assert.Equal(responses.Count, amount);
            Assert.Distinct(responses.Select(v => v.VoucherCode));
        }

        [Fact(DisplayName = "CreateVouchers throws ApiException when no product is found")]
        public async void CreateVouchersThrowsApiExceptionWhenNoProductIsFound()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(CreateVouchersHaveGivenLength)
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

            var voucherService = new VoucherService(context);

            // Act
            var request = new IssueVoucherRequest { ProductId = 1, Amount = 10 };

            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                voucherService.CreateVouchers(request)
            );
        }

        [Fact(DisplayName = "CreateVouchers have length of 8 + 4 from userPrefix")]
        public async void CreateVouchersHaveGivenLength()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(CreateVouchersHaveGivenLength)
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

            var product = new Product
            {
                Id = 1,
                Name = "product",
                Description = "desc",
            };
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();
            var voucherService = new VoucherService(context);

            // Act
            var request = new IssueVoucherRequest
            {
                ProductId = 1,
                Amount = 10,
                VoucherPrefix = "ABC",
            };
            var response = await voucherService.CreateVouchers(request);

            const int codeLength = 4 + 8; //ABC-XXXXXXXX

            Assert.All(
                response,
                voucherResponse => Assert.Equal(codeLength, voucherResponse.VoucherCode.Length)
            );
        }

        [Fact(DisplayName = "CreateVouchers saves to database")]
        public async void CreateVouchersSavesToDatabase()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(CreateVouchersSavesToDatabase)
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

            var product = new Product
            {
                Id = 1,
                Name = "product",
                Description = "desc",
            };
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();
            var voucherService = new VoucherService(context);

            // Act
            var request = new IssueVoucherRequest { ProductId = 1, Amount = 10 };
            var response = await voucherService.CreateVouchers(request);

            Assert.All(
                response,
                voucherResponse =>
                    Assert.True(context.Vouchers.Any(v => v.Code == voucherResponse.VoucherCode))
            );
        }

        [Fact(DisplayName = "All generated vouchers start with user chosen Prefix")]
        public async void CreateVouchersCreatesVouchersWithPrefix()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(CreateVouchersCreatesVouchersWithPrefix)
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

            var product = new Product
            {
                Id = 1,
                Name = "product",
                Description = "desc",
            };
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();
            var voucherService = new VoucherService(context);

            //Act
            var request = new IssueVoucherRequest
            {
                ProductId = 1,
                Amount = 10,
                VoucherPrefix = "ACT",
            };
            var response = await voucherService.CreateVouchers(request);

            //Assert
            Assert.All(
                response,
                voucherResponse => Assert.StartsWith("ACT-", voucherResponse.VoucherCode)
            );
        }
    }
}
