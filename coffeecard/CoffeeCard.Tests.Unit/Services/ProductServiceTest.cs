using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services
{
    public class ProductServiceTest
    {
        private static User testuser => new User(
            email: "test",
            name: "test",
            password: "pass",
            salt: "salt",
            programme: new Programme(fullName: "fullName", shortName: "shortName")
        );

        private static Product testProduct1 => new Product(
            id: 1,
            name: "Coffee",
            description: "Coffee clip card",
            numberOfTickets: 10,
            price: 10,
            experienceWorth: 10,
            visible: true,
            productUserGroup: new List<ProductUserGroup>()
        );

        private static Product testProduct2 => new Product(
            id: 2,
            name: "Espresso",
            description: "Espresso clip card",
            numberOfTickets: 10,
            price: 20,
            experienceWorth: 20,
            visible: true,
            productUserGroup: new List<ProductUserGroup>()
        );

        private static Product testProduct3 => new Product(
            id: 3,
            name: "Latte",
            description: "Latte clip card",
            numberOfTickets: 10,
            price: 30,
            experienceWorth: 30,
            visible: true,
            productUserGroup: new List<ProductUserGroup>()
        );

        [Fact(DisplayName = "GetProductsForUserAsync does not return non-visible products")]
        public async Task GetProductsForUserAsync_DoesNot_Return_NonVisible_Products()
        {
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetProductsForUserAsync_DoesNot_Return_NonVisible_Products));

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            var p1 = testProduct1;
            await context.AddAsync(p1);

            var p2 = testProduct2;
            p2.Visible = false;
            await context.AddAsync(p2);
            await context.SaveChangesAsync();

            await context.AddAsync(new ProductUserGroup(p1, UserGroup.Barista));
            await context.AddAsync(new ProductUserGroup(p2, UserGroup.Barista));
            await context.SaveChangesAsync();

            using var productService = new ProductService(context);
            var expected = new List<Product> { testProduct1 };

            var user = testuser;
            user.UserGroup = UserGroup.Barista;

            var result = await productService.GetProductsForUserAsync(user);

            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "GetProductsForUserAsync return products for usergroup")]
        public async Task GetProductsForUserAsync_Return_Products_For_UserGroup()
        {
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetProductsForUserAsync_Return_Products_For_UserGroup));

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            var p1 = testProduct1;
            await context.AddAsync(p1);

            var p2 = testProduct2;
            await context.AddAsync(p2);

            var p3 = testProduct3;
            await context.AddAsync(p3);
            await context.SaveChangesAsync();

            await context.AddAsync(new ProductUserGroup(p1, UserGroup.Barista));
            await context.AddAsync(new ProductUserGroup(p2, UserGroup.Barista));
            await context.AddAsync(new ProductUserGroup(p3, UserGroup.Barista));
            await context.SaveChangesAsync();

            using (var productService = new ProductService(context))
            {
                var expected = new List<Product>
                {
                    testProduct1,
                    testProduct2,
                    testProduct3
                };

                var user = testuser;
                user.UserGroup = UserGroup.Barista;

                var result = await productService.GetProductsForUserAsync(user);

                Assert.Equal(expected, result);
            }
        }

        [Fact(DisplayName = "GetPublicProducts does not return non-visible products")]
        public async Task GetPublicProducts_DoesNot_Return_NonVisible_Products()
        {
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetPublicProducts_DoesNot_Return_NonVisible_Products));

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            using (var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings))
            {
                var p1 = testProduct1;
                await context.AddAsync(p1);

                var p2 = testProduct2;
                p2.Visible = false;
                await context.AddAsync(p2);
                await context.SaveChangesAsync();

                await context.AddAsync(new ProductUserGroup(p1, UserGroup.Customer));
                await context.AddAsync(new ProductUserGroup(p2, UserGroup.Customer));
                await context.SaveChangesAsync();

                using var productService = new ProductService(context);
                var expected = new List<Product> { testProduct1 };

                var result = await productService.GetPublicProducts();

                Assert.Equal(expected, result);
            }
        }

        [Fact(DisplayName = "GetPublicProducts return all non-barista products")]
        public async Task GetPublicProducts_Return_All_NonBarista_Products()
        {
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetPublicProducts_Return_All_NonBarista_Products));

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            using (var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings))
            {
                var p1 = testProduct1;
                await context.AddAsync(p1);

                var p2 = testProduct2;
                await context.AddAsync(p2);

                var p3 = testProduct3;
                await context.AddAsync(p3);
                await context.SaveChangesAsync();

                await context.AddAsync(new ProductUserGroup(p1, UserGroup.Customer));
                await context.AddAsync(new ProductUserGroup(p2, UserGroup.Customer));
                await context.AddAsync(new ProductUserGroup(p3, UserGroup.Barista));
                await context.SaveChangesAsync();

                using var productService = new ProductService(context);
                var expected = new List<Product>
                {
                    testProduct1,
                    testProduct2
                };

                var result = await productService.GetPublicProducts();

                Assert.Equal(expected, result);
            }
        }
    }
}