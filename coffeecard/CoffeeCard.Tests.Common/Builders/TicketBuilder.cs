using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    [BuilderFor(typeof(Ticket))]
    public partial class TicketBuilder
    {
        public static TicketBuilder Simple()
        {
            var owner = UserBuilder.Simple().Build();
            var purchase = PurchaseBuilder.Simple().Build();
            return new TicketBuilder().WithOwner(owner).WithPurchase(purchase);
        }

        public static TicketBuilder Typical()
        {
            return Simple();
        }
    }
}
