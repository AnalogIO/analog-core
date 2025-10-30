using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    [BuilderFor(typeof(Statistic))]
    public partial class StatisticBuilder
    {
        public static StatisticBuilder Simple()
        {
            var user = UserBuilder.Simple().Build();
            return new StatisticBuilder().WithUser(user);
        }

        public static StatisticBuilder Typical()
        {
            return Simple();
        }
    }
}
