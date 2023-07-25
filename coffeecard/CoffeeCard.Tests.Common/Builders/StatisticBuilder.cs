using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    public class StatisticBuilder : BaseBuilder<Statistic>
    {
        public override StatisticBuilder Simple()
        {
			var user = new UserBuilder().Simple().Build();
            Faker.RuleFor(p => p.Id, f => f.IndexGlobal)
                .RuleFor(p => p.User, user);
            return this;
        }

        public override StatisticBuilder Typical()
        {
            return Simple();
        }
    }
}