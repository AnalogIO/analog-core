using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    public class ProgrammeBuilder : BaseBuilder<Programme>
    {
        public override ProgrammeBuilder Simple()
        {
            Faker.RuleFor(p => p.Id, f => f.IndexGlobal)
                .RuleFor(p => p.FullName, f => f.Commerce.Department())
                .RuleFor(p => p.ShortName, f => f.Random.String2(3))
                .RuleFor(p => p.Users, () => new List<User>());
            return this;
        }

        public override ProgrammeBuilder Typical()
        {
            return Simple();
        }
    }
}