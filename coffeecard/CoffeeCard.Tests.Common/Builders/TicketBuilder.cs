using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    public class TicketBuilder : BaseBuilder<Ticket>
    {
        public override TicketBuilder Simple()
        {
            var owner = new UserBuilder().Simple().Build();
            var purchase = new PurchaseBuilder().Simple().Build();
            Faker.RuleFor(p => p.Id, f => f.IndexGlobal)
                .RuleFor(p => p.Owner, owner)
                .RuleFor(p => p.OwnerId, owner.Id)
                .RuleFor(p => p.Purchase, f => purchase)
                .RuleFor(p => p.PurchaseId, purchase.Id);

            return this;
        }

        public override TicketBuilder Typical()
        {
            return Simple();
        }
    }
}