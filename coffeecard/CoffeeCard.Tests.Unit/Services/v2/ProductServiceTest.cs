using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Models.DataTransferObjects.v2.Product;
using CoffeeCard.Models.DataTransferObjects.v2.Products;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services.v2
{
    public class ProductServiceTest
    {
        [Fact(DisplayName = "UpdateProduct removes ommitted user groups and only adds selected user groups")]
        public async Task UpdateProduct_Removes_Omitted_UserGroups()
        {
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(UpdateProduct_Removes_Omitted_UserGroups));

            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            Product p = new Product
            {
                Id = 1,
                Name = "Coffee",
                Description = "Coffee Clip card",
                NumberOfTickets = 10,
                Price = 10,
                ExperienceWorth = 10,
                Visible = true
            };
            _ = await context.AddAsync(p);
            _ = await context.SaveChangesAsync();

            _ = await context.AddAsync(new ProductUserGroup
            {
                Product = p,
                UserGroup = UserGroup.Barista
            });

            _ = await context.AddAsync(new ProductUserGroup
            {
                Product = p,
                UserGroup = UserGroup.Manager
            });

            _ = await context.SaveChangesAsync();

            using ProductService productService = new ProductService(context);

            _ = await productService.UpdateProduct(1, new UpdateProductRequest()
            {
                Visible = true,
                Price = 10,
                NumberOfTickets = 10,
                Name = "Coffee",
                Description = "Coffee Clip card",
                AllowedUserGroups = [UserGroup.Customer, UserGroup.Board]
            });

            ProductResponse result = await productService.GetProductAsync(1);

            Assert.Collection<UserGroup>(result.AllowedUserGroups,
                e => Assert.Equal(UserGroup.Customer, e),
                e => Assert.Equal(UserGroup.Board, e));

            // Explicitly check for exclusion of Barista and Manager, even though Assert.Collection implicitly covers it.
            Assert.DoesNotContain(UserGroup.Barista, result.AllowedUserGroups);
            Assert.DoesNotContain(UserGroup.Manager, result.AllowedUserGroups);
        }

        [Fact(DisplayName = "AddProduct adds only selected user groups")]
        public async Task AddProduct_Sets_Correct_UserGroups()
        {
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(AddProduct_Sets_Correct_UserGroups));

            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);

            using ProductService productService = new ProductService(context);

            AddProductRequest p = new AddProductRequest
            {
                Name = "Coffee",
                Description = "Coffee Clip card",
                NumberOfTickets = 10,
                Price = 10,
                Visible = true,
                AllowedUserGroups = [UserGroup.Manager, UserGroup.Board]
            };

            _ = await productService.AddProduct(p);

            ProductResponse result = await productService.GetProductAsync(1);

            Assert.Collection<UserGroup>(result.AllowedUserGroups,
                e => Assert.Equal(UserGroup.Manager, e),
                e => Assert.Equal(UserGroup.Board, e));
        }

        [Fact(DisplayName = "GetAllProducts shows non-visible products")]
        public async Task GetAllProducts_Returns_Non_Visible_Products()
        {
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetAllProducts_Returns_Non_Visible_Products));

            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);

            using ProductService productService = new ProductService(context);

            AddProductRequest p1 = new AddProductRequest
            {
                Name = "Coffee",
                Description = "Coffee Clip card",
                NumberOfTickets = 10,
                Price = 10,
                Visible = true,
                AllowedUserGroups = Enum.GetValues<UserGroup>()
            };
            _ = await productService.AddProduct(p1);

            AddProductRequest p2 = new AddProductRequest
            {
                Name = "Latte",
                Description = "Fancy Drink Clip card",
                NumberOfTickets = 10,
                Price = 170,
                Visible = false,
                AllowedUserGroups = Enum.GetValues<UserGroup>()
            };
            _ = await productService.AddProduct(p2);

            IEnumerable<ProductResponse> result = await productService.GetAllProductsAsync();

            Assert.Collection(result,
                e => Assert.True(e.Visible),
                e => Assert.False(e.Visible));
        }

        [Fact(DisplayName = "GetAllProducts returns products from all user groups")]
        public async Task GetAllProducts_Returns_Products_For_All_UserGroups()
        {
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetAllProducts_Returns_Products_For_All_UserGroups));

            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);

            using ProductService productService = new ProductService(context);

            AddProductRequest p1 = new AddProductRequest
            {
                Name = "Coffee",
                Description = "Coffee Clip card",
                NumberOfTickets = 10,
                Price = 10,
                Visible = true,
                AllowedUserGroups = [UserGroup.Customer]
            };
            _ = await productService.AddProduct(p1);

            AddProductRequest p2 = new AddProductRequest
            {
                Name = "Latte",
                Description = "Fancy Drink Clip card",
                NumberOfTickets = 10,
                Price = 170,
                Visible = true,
                AllowedUserGroups = [UserGroup.Barista]
            };
            _ = await productService.AddProduct(p2);

            AddProductRequest p3 = new AddProductRequest
            {
                Name = "Frappuccino",
                Description = "Blended ice with sugar",
                NumberOfTickets = 1,
                Price = 35,
                Visible = true,
                AllowedUserGroups = [UserGroup.Manager]
            };
            _ = await productService.AddProduct(p3);

            AddProductRequest p4 = new AddProductRequest
            {
                Name = "Cortado",
                Description = "Some spanish coffee",
                NumberOfTickets = 1,
                Price = 19,
                Visible = true,
                AllowedUserGroups = [UserGroup.Board]
            };
            _ = await productService.AddProduct(p4);

            IEnumerable<ProductResponse> result = await productService.GetAllProductsAsync();

            Assert.Collection(result,
                e =>
                {
                    _ = Assert.Single(e.AllowedUserGroups);
                    Assert.Contains(UserGroup.Customer, e.AllowedUserGroups);
                },
                e =>
                {
                    _ = Assert.Single(e.AllowedUserGroups);
                    Assert.Contains(UserGroup.Barista, e.AllowedUserGroups);
                },
                e =>
                {
                    _ = Assert.Single(e.AllowedUserGroups);
                    Assert.Contains(UserGroup.Manager, e.AllowedUserGroups);
                },
                e =>
                {
                    _ = Assert.Single(e.AllowedUserGroups);
                    Assert.Contains(UserGroup.Board, e.AllowedUserGroups);
                }
            );
        }
    }
}