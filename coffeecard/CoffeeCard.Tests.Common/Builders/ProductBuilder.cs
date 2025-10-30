using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    [BuilderFor(typeof(Product))]
    public partial class ProductBuilder
    {
        public static ProductBuilder Simple()
        {
            return new ProductBuilder()
                .WithPrice(f => f.Random.Int(0, 200))
                .WithVisible(true)
                .WithExperienceWorth(f => f.Random.Int(0, 200))
                .WithName(f => f.Commerce.ProductName())
                .WithDescription(f => f.Commerce.ProductDescription())
                .WithNumberOfTickets(f => f.PickRandom(1, 10))
                .WithEligibleMenuItems([])
                .WithMenuItemProducts([])
                .WithProductUserGroup([]);
        }

        public static ProductBuilder Typical()
        {
            return Simple()
                .WithMenuItemProducts(MenuItemProductBuilder.Simple().Build(1))
                .WithEligibleMenuItems(MenuItemBuilder.Simple().Build(1))
                .WithProductUserGroup(ProductUserGroupBuilder.Simple().Build(1));
        }
    }
}
