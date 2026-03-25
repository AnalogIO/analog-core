using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    [BuilderFor(typeof(Voucher))]
    public partial class VoucherBuilder
    {
        public static VoucherBuilder Simple()
        {
            var product = ProductBuilder.Simple().Build();
            return new VoucherBuilder()
                .WithProduct(product)
                .WithPurchase(_ => null)
                .WithUser(_ => null)
                .WithDescription(_ => null)
                .WithRequester(_ => null);
        }

        public static VoucherBuilder Typical()
        {
            return Simple()
                .WithPurchase(PurchaseBuilder.Simple().WithType(PurchaseType.Voucher).Build())
                .WithUser(UserBuilder.DefaultCustomer().Build());
        }
    }
}
