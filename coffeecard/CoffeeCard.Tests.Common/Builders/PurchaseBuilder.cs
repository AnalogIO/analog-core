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
                .WithTickets(new List<Ticket>())
                .WithNumberOfTickets(0);
        }

        public static PurchaseBuilder Typical()
        {
            return Simple();
        }
    }
}
