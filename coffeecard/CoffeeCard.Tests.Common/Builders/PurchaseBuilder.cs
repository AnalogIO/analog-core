using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    [BuilderFor(typeof(Purchase))]
    public partial class PurchaseBuilder
    {
        public static PurchaseBuilder Simple()
        {
            User purchasedBy = UserBuilder.Simple().Build();
            return new PurchaseBuilder().WithPurchasedBy(purchasedBy)
                .WithProductName(f => f.Commerce.ProductName())
                .WithTickets([])
                .WithTickets([])
                .WithNumberOfTickets(0);
        }

        public static PurchaseBuilder Typical()
        {
            return Simple();
        }
    }
}