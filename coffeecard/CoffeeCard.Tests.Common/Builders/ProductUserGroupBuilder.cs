using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    public class ProductUserGroupBuilder : BaseBuilder<ProductUserGroup>
    {
        public override ProductUserGroupBuilder Simple()
        {
            var product = new ProductBuilder().Simple().Build();
            Faker.RuleFor(p => p.Product, product)
            .RuleFor(p => p.ProductId, product.Id)
    .RuleFor(p => p.UserGroup, f => f.PickRandom(UserGroup.Barista, UserGroup.Board, UserGroup.Customer, UserGroup.Manager));
            return this;
        }

        public override ProductUserGroupBuilder Typical()
        {
            return Simple();
        }
    }
}