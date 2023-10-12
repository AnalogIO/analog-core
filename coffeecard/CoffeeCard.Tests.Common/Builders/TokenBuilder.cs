using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    [BuilderFor(typeof(Token))]
    public partial class TokenBuilder
    {
        public static TokenBuilder Simple()
        {
            return new TokenBuilder()
                //TODO id?
                .WithUser(UserBuilder.Simple().Build());
        }

        public static TokenBuilder Typical()
        {
            return Simple();
        }
    }
}