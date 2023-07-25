using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    public class VoucherBuilder : BaseBuilder<Voucher>
    {
        public override VoucherBuilder Simple()
        {
			var product = new ProductBuilder().Simple().Build();
            Faker.RuleFor(p => p.Id, f => f.IndexGlobal)
                .RuleFor(p => p.User, f => null)
				.RuleFor(v => v.UserId, f => null)
				.RuleFor(v => v.Product, product)
				.RuleFor(v => v.ProductId, product.Id);
            return this;
        }

        public override VoucherBuilder Typical()
        {
            return Simple();
        }
    }
}