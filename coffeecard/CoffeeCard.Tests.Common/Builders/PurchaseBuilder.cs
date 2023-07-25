using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    public class PurchaseBuilder : BaseBuilder<Purchase>
    {
        public override PurchaseBuilder Simple()
        {
            var purchasedBy = new UserBuilder().Simple().Build();
            Faker.RuleFor(p => p.Id, f => f.IndexGlobal)
                .RuleFor(p => p.PurchasedBy, purchasedBy)
                .RuleFor(p => p.PurchasedById, purchasedBy.Id)
                .RuleFor(p => p.Tickets, new List<Ticket>())
                .RuleFor(p => p.NumberOfTickets, 0);
            return this;
        }

        public override PurchaseBuilder Typical()
        {
            return Simple();
        }
    }
}