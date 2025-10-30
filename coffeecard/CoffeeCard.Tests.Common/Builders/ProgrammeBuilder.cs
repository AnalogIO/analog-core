using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    [BuilderFor(typeof(Programme))]
    public partial class ProgrammeBuilder
    {
        public static ProgrammeBuilder Simple()
        {
            return new ProgrammeBuilder()
                .WithUsers(new List<User>())
                .WithSortPriority(f => f.Random.Int(min: 0, max: 100))
                .WithShortName(f => f.Random.String2(3))
                .WithFullName(f => f.Commerce.Department());
        }

        public static ProgrammeBuilder Typical()
        {
            return Simple();
        }
    }
}
