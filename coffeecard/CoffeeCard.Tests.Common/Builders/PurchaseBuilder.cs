using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    [BuilderFor(typeof(Purchase))]
    public partial class PurchaseBuilder
    {
        public static PurchaseBuilder Simple()
        {
            var purchasedBy = UserBuilder.Simple().Build();
            return new PurchaseBuilder()
                .WithPurchasedBy(purchasedBy)
                .WithProductName(f => f.Commerce.ProductName())
                .WithTickets(new List<Ticket>())
                .WithNumberOfTickets(0)
                .WithVoucher(_ => null)
                .WithProduct(ProductBuilder.Simple().Build())
                .WithPrice(f => f.Random.Int(1, 200))
                .WithDateCreated(f => f.Date.Past())
                .WithStatus(f => f.Random.Enum<PurchaseStatus>())
                .WithType(f => f.Random.Enum<PurchaseType>())
                .WithOrderId(f => f.Random.Guid().ToString());
        }

        public static PurchaseBuilder Typical()
        {
            return Simple();
        }
    }
}
