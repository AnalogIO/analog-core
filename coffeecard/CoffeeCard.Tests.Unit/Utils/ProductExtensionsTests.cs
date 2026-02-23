using System;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.v2.Products;
using CoffeeCard.Models.Entities;
using CoffeeCard.Tests.Common.Builders;
using Xunit;

namespace CoffeeCard.Tests.Unit.Utils
{
    public class ProductExtensionsTests
    {
        [Fact(
            DisplayName = "IsPerk returns true if the product is not valid for regular customers"
        )]
        public void TestIsPerkReturnsTrue()
        {
            var product = ProductBuilder
                .Simple()
                .WithProductUserGroup(
                    [
                        new ProductUserGroup { UserGroup = UserGroup.Manager },
                        new ProductUserGroup { UserGroup = UserGroup.Board },
                    ]
                )
                .Build();

            Assert.True(product.IsPerk());
        }

        [Fact(DisplayName = "IsPerk returns false if the product is valid for regular customers")]
        public void TestIsPerkReturnsFalse()
        {
            var product = ProductBuilder
                .Simple()
                .WithProductUserGroup([new ProductUserGroup { UserGroup = UserGroup.Customer }])
                .Build();

            Assert.False(product.IsPerk());
        }

        [Fact(DisplayName = "IsPerk throws ArgumentNullException if ProductUserGroup is null")]
        public void TestIsPerkThrowsArgumentNullException()
        {
            var product = ProductBuilder.Simple().WithProductUserGroup(_ => null).Build();

            Assert.Throws<ArgumentNullException>(() => product.IsPerk());
        }

        [Fact(DisplayName = "ToProductResponse returns a ProductResponse with the correct values")]
        public void TestToProductResponse()
        {
            var product = ProductBuilder.Typical().Build();

            var productResponse = product.ToProductResponse();

            Assert.IsType<ProductResponse>(productResponse);
        }

        [Fact(
            DisplayName = "ToProductResponse converts ProductUserGroup to list of allowed user groups"
        )]
        public void TestToProductResponseConvertsProductUserGroup()
        {
            var product = new Product
            {
                Id = 1,
                Name = "Coffee",
                Description = "Coffee Clip card",
                NumberOfTickets = 10,
                Price = 10,
                ExperienceWorth = 10,
                Visible = true,
                ProductUserGroup = new[]
                {
                    new ProductUserGroup { UserGroup = UserGroup.Manager },
                    new ProductUserGroup { UserGroup = UserGroup.Board },
                },
                EligibleMenuItems = new[]
                {
                    new MenuItem { Id = 1, Name = "Coffee" },
                },
            };

            var productResponse = product.ToProductResponse();

            Assert.Collection(
                productResponse.AllowedUserGroups,
                userGroup => Assert.Equal(UserGroup.Manager, userGroup),
                userGroup => Assert.Equal(UserGroup.Board, userGroup)
            );
        }

        [Fact(
            DisplayName = "ToProductResponse converts EligibleMenuItems to list of MenuItemResponse"
        )]
        public void TestToProductResponseConvertsEligibleMenuItems()
        {
            var product = ProductBuilder
                .Typical()
                .WithEligibleMenuItems(
                    [
                        new MenuItem { Id = 1, Name = "Coffee" },
                        new MenuItem { Id = 2, Name = "Tea" },
                    ]
                )
                .Build();

            var productResponse = product.ToProductResponse();

            Assert.Collection(
                productResponse.EligibleMenuItems,
                menuItem =>
                {
                    Assert.Equal(1, menuItem.Id);
                    Assert.Equal("Coffee", menuItem.Name);
                },
                menuItem =>
                {
                    Assert.Equal(2, menuItem.Id);
                    Assert.Equal("Tea", menuItem.Name);
                }
            );
        }
    }
}
