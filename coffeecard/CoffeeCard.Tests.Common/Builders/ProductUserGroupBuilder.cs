using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    [BuilderFor(typeof(ProductUserGroup))]
    public partial class ProductUserGroupBuilder
    {
        public static ProductUserGroupBuilder Simple()
        {
            var product = ProductBuilder.Simple().Build();
            return new ProductUserGroupBuilder()
                .WithProduct(product)
                .WithUserGroup(UserGroup.Customer);
        }

        public static ProductUserGroupBuilder Typical()
        {
            return Simple();
        }
    }
}
