using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.Entities;
using CoffeeCard.Tests.Common.Builders;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services
{
    public class ProductServiceTest : BaseUnitTests
    {
        [Fact(DisplayName = "GetProductsForUserAsync does not return non-visible products")]
        public async Task GetProductsForUserAsync_DoesNot_Return_NonVisible_Products()
        {
            var p1 = ProductBuilder.Simple().WithVisible(true).Build();
            var p2 = ProductBuilder.Simple().WithVisible(false).Build();

            IList<ProductUserGroup> productUserGroups =
            [
                ProductUserGroupBuilder
                    .Simple()
                    .WithUserGroup(UserGroup.Barista)
                    .WithProduct(p1)
                    .Build(),
                ProductUserGroupBuilder
                    .Simple()
                    .WithUserGroup(UserGroup.Barista)
                    .WithProduct(p2)
                    .Build(),
            ];

            await InitialContext.ProductUserGroups.AddRangeAsync(productUserGroups);
            await InitialContext.SaveChangesAsync();

            using var productService = new ProductService(InitialContext);
            IList<Product> expected = [p1];

            var user = UserBuilder.Simple().WithUserGroup(UserGroup.Barista).Build();

            var result = await productService.GetProductsForUserAsync(user);

            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "GetProductsForUserAsync return products for usergroup")]
        public async Task GetProductsForUserAsync_Return_Products_For_UserGroup()
        {
            var products = ProductBuilder.Simple().Build(3);
            var pugs = ProductUserGroupBuilder.Simple().WithUserGroup(UserGroup.Barista).Build(3);

            for (var i = 0; i < products.Count; i++)
            {
                products[i].ProductUserGroup = [pugs[i]];
                pugs[i].Product = products[i];
            }

            await InitialContext.AddRangeAsync(products);
            await InitialContext.SaveChangesAsync();

            using var productService = new ProductService(AssertionContext);

            var user = UserBuilder.Simple().WithUserGroup(UserGroup.Barista).Build();

            var result = await productService.GetProductsForUserAsync(user);
            var expected = products;

            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "GetPublicProducts does not return non-visible products")]
        public async Task GetPublicProducts_DoesNot_Return_NonVisible_Products()
        {
            IList<Product> products =
            [
                ProductBuilder.Simple().WithVisible(true).Build(),
                ProductBuilder.Simple().WithVisible(false).Build(),
            ];
            var pugs = ProductUserGroupBuilder.Simple().WithUserGroup(UserGroup.Customer).Build(2);
            for (var i = 0; i < products.Count; i++)
            {
                products[i].ProductUserGroup = [pugs[i]];
                pugs[i].Product = products[i];
            }

            await InitialContext.AddRangeAsync(products);
            await InitialContext.SaveChangesAsync();

            using var productService = new ProductService(AssertionContext);

            var result = await productService.GetPublicProducts();

            var expected = new List<Product>
            {
                // We only expect the first element, since that is the only visible one
                products.First(),
            };
            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "GetPublicProducts return all non-barista products")]
        public async Task GetPublicProducts_Return_All_NonBarista_Products()
        {
            var products = ProductBuilder.Simple().Build(3);
            var pugs = ProductUserGroupBuilder
                .Simple()
                .WithUserGroup(UserGroup.Customer)
                .Build(2)
                .Concat(ProductUserGroupBuilder.Simple().WithUserGroup(UserGroup.Barista).Build(1))
                .ToList();
            for (var i = 0; i < products.Count; i++)
            {
                products[i].ProductUserGroup = [pugs[i]];
                pugs[i].Product = products[i];
            }

            await InitialContext.AddRangeAsync(products);
            await InitialContext.SaveChangesAsync();

            using var productService = new ProductService(AssertionContext);
            var expected = products.Take(2);

            var result = await productService.GetPublicProducts();

            Assert.Equal(expected, result);
        }
    }
}
