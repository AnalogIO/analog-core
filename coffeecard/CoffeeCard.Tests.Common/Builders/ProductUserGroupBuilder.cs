using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    [BuilderFor(typeof(ProductUserGroup))]
    public partial class ProductUserGroupBuilder
    {
        public static ProductUserGroupBuilder Simple()
        {
            Product product = ProductBuilder.Simple().Build();
            return new ProductUserGroupBuilder()
                .WithProduct(product)
                .WithUserGroup(f =>
                    f.PickRandom(UserGroup.Barista, UserGroup.Board, UserGroup.Customer, UserGroup.Manager));
        }

        public static ProductUserGroupBuilder Typical()
        {
            return Simple();
        }
    }
}