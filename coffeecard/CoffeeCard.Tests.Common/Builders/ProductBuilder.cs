using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    public class ProductBuilder : BaseBuilder<Product>
    {
        public override ProductBuilder Simple()
        {
            Faker.RuleFor(p => p.Id, f => f.IndexGlobal)
				.RuleFor(p => p.Name, f => f.Commerce.Product())
				.RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
				.RuleFor(p => p.NumberOfTickets, f => f.PickRandom(1, 10))
				.RuleFor(p => p.ProductUserGroup, new List<ProductUserGroup>());
            return this;
        }

        public override ProductBuilder Typical()
        {
            return Simple();
        }
    }
}