using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            var expected = new List<UserGroup>
            {
                UserGroup.Customer, UserGroup.Board
            };

            var result = await productService.GetProductAsync(1);

            Assert.Collection<UserGroup>(expected,
                e => e.Equals(UserGroup.Customer),
                e => e.Equals(UserGroup.Board));
        }
    }
}