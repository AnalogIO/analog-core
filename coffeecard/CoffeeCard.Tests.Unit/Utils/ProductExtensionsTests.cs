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
                        ProductUserGroupBuilder.Simple().WithUserGroup(UserGroup.Manager).Build(),
                        ProductUserGroupBuilder.Simple().WithUserGroup(UserGroup.Board).Build(),
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
                .WithProductUserGroup(
                    [ProductUserGroupBuilder.Simple().WithUserGroup(UserGroup.Customer).Build()]
                )
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
            var product = ProductBuilder
                .Simple()
                .WithProductUserGroup(
                    [
                        ProductUserGroupBuilder.Simple().WithUserGroup(UserGroup.Manager).Build(),
                        ProductUserGroupBuilder.Simple().WithUserGroup(UserGroup.Board).Build(),
                    ]
                )
                .Build();

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
