using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Models.DataTransferObjects.v2.Voucher;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(CreateVouchersReturnsUniqueResponses) +
                                     amount + productId);
            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);

            Product product = new Product { Id = 1, Name = "product", Description = "desc" };
            _ = await context.Products.AddAsync(product);
            _ = await context.SaveChangesAsync();
            VoucherService voucherService = new VoucherService(context);

            // Act
            IssueVoucherRequest request = new IssueVoucherRequest { Amount = amount, ProductId = productId };
            System.Collections.Generic.List<IssueVoucherResponse> responses = (await voucherService.CreateVouchers(request)).ToList();

            // Assert
            Assert.Equal(responses.Count, amount);
            Assert.Distinct(responses.Select(v => v.VoucherCode));
        }

        [Fact(DisplayName = "CreateVouchers throws ApiException when no product is found")]
        public async void CreateVouchersThrowsApiExceptionWhenNoProductIsFound()
        {
            // Arrange
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(CreateVouchersHaveGivenLength));
            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);

            VoucherService voucherService = new VoucherService(context);

            // Act
            IssueVoucherRequest request = new IssueVoucherRequest { ProductId = 1, Amount = 10 };

            _ = await Assert.ThrowsAsync<EntityNotFoundException>(() => voucherService.CreateVouchers(request));
        }

        [Fact(DisplayName = "CreateVouchers have length of 8 + 4 from userPrefix")]
        public async void CreateVouchersHaveGivenLength()
        {
            // Arrange
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(CreateVouchersHaveGivenLength));
            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);

            Product product = new Product { Id = 1, Name = "product", Description = "desc" };
            _ = await context.Products.AddAsync(product);
            _ = await context.SaveChangesAsync();
            VoucherService voucherService = new VoucherService(context);

            // Act
            IssueVoucherRequest request = new IssueVoucherRequest { ProductId = 1, Amount = 10, VoucherPrefix = "ABC" };
            System.Collections.Generic.IEnumerable<IssueVoucherResponse> response = await voucherService.CreateVouchers(request);

            const int codeLength = 4 + 8; //ABC-XXXXXXXX

            Assert.All(response, voucherResponse => Assert.Equal(codeLength, voucherResponse.VoucherCode.Length));
        }

        [Fact(DisplayName = "CreateVouchers saves to database")]
        public async void CreateVouchersSavesToDatabase()
        {
            // Arrange
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(CreateVouchersSavesToDatabase));
            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);

            Product product = new Product { Id = 1, Name = "product", Description = "desc" };
            _ = await context.Products.AddAsync(product);
            _ = await context.SaveChangesAsync();
            VoucherService voucherService = new VoucherService(context);

            // Act
            IssueVoucherRequest request = new IssueVoucherRequest { ProductId = 1, Amount = 10 };
            System.Collections.Generic.IEnumerable<IssueVoucherResponse> response = await voucherService.CreateVouchers(request);

            Assert.All(response, voucherResponse => Assert.True(context.Vouchers.Any(v => v.Code == voucherResponse.VoucherCode)));
        }

        [Fact(DisplayName = "All generated vouchers start with user chosen Prefix")]
        public async void CreateVouchersCreatesVouchersWithPrefix()
        {
            // Arrange
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(CreateVouchersCreatesVouchersWithPrefix));
            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);

            Product product = new Product { Id = 1, Name = "product", Description = "desc" };
            _ = await context.Products.AddAsync(product);
            _ = await context.SaveChangesAsync();
            VoucherService voucherService = new VoucherService(context);

            //Act
            IssueVoucherRequest request = new IssueVoucherRequest { ProductId = 1, Amount = 10, VoucherPrefix = "ACT" };
            System.Collections.Generic.IEnumerable<IssueVoucherResponse> response = await voucherService.CreateVouchers(request);

            //Assert
            Assert.All(response, voucherResponse => Assert.StartsWith("ACT-", voucherResponse.VoucherCode));
        }
    }
}