using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    public class TokenBuilder : BaseBuilder<Token>
    {
        public override TokenBuilder Simple()
        {
            Faker.RuleFor(p => p.Id, f => f.IndexGlobal)
                .RuleFor(p => p.User, new UserBuilder().Simple().Build());
            return this;
        }

        public override TokenBuilder Typical()
        {
            return Simple();
        }
    }
}