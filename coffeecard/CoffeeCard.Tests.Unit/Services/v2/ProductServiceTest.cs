using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Models.DataTransferObjects.v2.Product;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services.v2
{
    public class ProductServiceTest
    {
        [Fact(DisplayName = "UpdateProduct removes ommitted user groups and only adds selected user groups")]
        public async Task UpdateProduct_Removes_Omitted_UserGroups()
        {
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(UpdateProduct_Removes_Omitted_UserGroups));

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            var p = new Product
            {
                Id = 1,
                Name = "Coffee",
                Description = "Coffee Clip card",
                NumberOfTickets = 10,
                Price = 10,
                ExperienceWorth = 10,
                Visible = true
            };
            await context.AddAsync(p);
            await context.SaveChangesAsync();

            await context.AddAsync(new ProductUserGroup
            {
                Product = p,
                UserGroup = UserGroup.Barista
            });

            await context.AddAsync(new ProductUserGroup
            {
                Product = p,
                UserGroup = UserGroup.Manager
            });

            await context.SaveChangesAsync();

            using var productService = new ProductService(context);

            await productService.UpdateProduct(new UpdateProductRequest()
            {
                Id = 1,
                Visible = true,
                Price = 10,
                NumberOfTickets = 10,
                Name = "Coffee",
                Description = "Coffee Clip card",
                AllowedUserGroups = new List<UserGroup>() { UserGroup.Customer, UserGroup.Board }
            });

            var result = await productService.GetProductAsync(1);

            Assert.Collection<ProductUserGroup>(result.ProductUserGroup,
                e => Assert.Equal(UserGroup.Customer, e.UserGroup),
                e => Assert.Equal(UserGroup.Board, e.UserGroup));

            Assert.DoesNotContain(UserGroup.Barista, result.ProductUserGroup.Select(e => e.UserGroup));
            Assert.DoesNotContain(UserGroup.Manager, result.ProductUserGroup.Select(e => e.UserGroup));
        }

        [Fact(DisplayName = "AddProduct adds only selected user groups")]
        public async Task AddProduct_Sets_Correct_UserGroups()
        {
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(AddProduct_Sets_Correct_UserGroups));

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);

            using var productService = new ProductService(context);

            var p = new AddProductRequest
            {
                Name = "Coffee",
                Description = "Coffee Clip card",
                NumberOfTickets = 10,
                Price = 10,
                Visible = true,
                AllowedUserGroups = new List<UserGroup> { UserGroup.Manager, UserGroup.Board }
            };

            await productService.AddProduct(p);

            var result = await productService.GetProductAsync(1);

            Assert.Collection<ProductUserGroup>(result.ProductUserGroup,
                e => Assert.Equal(UserGroup.Manager, e.UserGroup),
                e => Assert.Equal(UserGroup.Board, e.UserGroup));
        }

        [Fact(DisplayName = "GetAllProducts shows non-visible products")]
        public async Task GetAllProducts_Returns_Non_Visible_Products()
        {
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetAllProducts_Returns_Non_Visible_Products));

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);

            using var productService = new ProductService(context);

            var p1 = new AddProductRequest
            {
                Name = "Coffee",
                Description = "Coffee Clip card",
                NumberOfTickets = 10,
                Price = 10,
                Visible = true,
                AllowedUserGroups = Enum.GetValues<UserGroup>()
            };
            await productService.AddProduct(p1);

            var p2 = new AddProductRequest
            {
                Name = "Latte",
                Description = "Fancy Drink Clip card",
                NumberOfTickets = 10,
                Price = 170,
                Visible = false,
                AllowedUserGroups = Enum.GetValues<UserGroup>()
            };
            await productService.AddProduct(p2);

            var result = await productService.GetAllProductsAsync();

            Assert.Collection(result,
                e => e.Visible.Equals(true),
                e => e.Visible.Equals(false));
        }

        [Fact(DisplayName = "GetAllProducts returns products from all user groups")]
        public async Task GetAllProducts_Returns_Products_For_All_UserGroups()
        {
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetAllProducts_Returns_Products_For_All_UserGroups));

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);

            using var productService = new ProductService(context);

            var p1 = new AddProductRequest
            {
                Name = "Coffee",
                Description = "Coffee Clip card",
                NumberOfTickets = 10,
                Price = 10,
                Visible = true,
                AllowedUserGroups = new List<UserGroup> { UserGroup.Customer }
            };
            await productService.AddProduct(p1);

            var p2 = new AddProductRequest
            {
                Name = "Latte",
                Description = "Fancy Drink Clip card",
                NumberOfTickets = 10,
                Price = 170,
                Visible = true,
                AllowedUserGroups = new List<UserGroup> { UserGroup.Barista }
            };
            await productService.AddProduct(p2);

            var p3 = new AddProductRequest
            {
                Name = "Frappuccino",
                Description = "Blended ice with sugar",
                NumberOfTickets = 1,
                Price = 35,
                Visible = true,
                AllowedUserGroups = new List<UserGroup> { UserGroup.Manager }
            };
            await productService.AddProduct(p3);

            var p4 = new AddProductRequest
            {
                Name = "Cortado",
                Description = "Some spanish coffee",
                NumberOfTickets = 1,
                Price = 19,
                Visible = true,
                AllowedUserGroups = new List<UserGroup> { UserGroup.Board }
            };
            await productService.AddProduct(p4);

            var result = await productService.GetAllProductsAsync();

            Assert.Collection<Product>(result,
                e => e.ProductUserGroup = new List<ProductUserGroup> { new ProductUserGroup { UserGroup = UserGroup.Customer } },
                e => e.ProductUserGroup = new List<ProductUserGroup> { new ProductUserGroup { UserGroup = UserGroup.Barista } },
                e => e.ProductUserGroup = new List<ProductUserGroup> { new ProductUserGroup { UserGroup = UserGroup.Manager } },
                e => e.ProductUserGroup = new List<ProductUserGroup> { new ProductUserGroup { UserGroup = UserGroup.Board } }
            );
        }
    }
}