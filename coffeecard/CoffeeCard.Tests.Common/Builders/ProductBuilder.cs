using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    [BuilderFor(typeof(Product))]
    public partial class ProductBuilder
    {
        public static ProductBuilder Simple()
        {
            return new ProductBuilder()
                .WithName(f => f.Commerce.ProductName())
                .WithDescription(f => f.Commerce.ProductDescription())
                .WithNumberOfTickets(f => f.PickRandom(1, 10))
                .WithProductUserGroup(new List<ProductUserGroup>());
        }

        public static ProductBuilder Typical()
        {
            return Simple();
        }
    }
}