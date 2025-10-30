using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    [BuilderFor(typeof(Voucher))]
    public partial class VoucherBuilder
    {
        public static VoucherBuilder Simple()
        {
            var product = ProductBuilder.Simple().Build();
            return new VoucherBuilder().WithProduct(product).WithUser(f => null);
        }

        public static VoucherBuilder Typical()
        {
            return Simple();
        }
    }
}
