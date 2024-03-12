using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    [BuilderFor(typeof(Programme))]
    public partial class ProgrammeBuilder
    {
        public static ProgrammeBuilder Simple()
        {
            return new ProgrammeBuilder()
                .WithUsers([])
                .WithShortName(f => f.Random.String2(3))
                .WithFullName(f => f.Commerce.Department());
        }

        public static ProgrammeBuilder Typical()
        {
            return Simple();
        }
    }
}