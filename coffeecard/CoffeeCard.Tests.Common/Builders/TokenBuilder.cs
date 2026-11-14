using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    [BuilderFor(typeof(Token))]
    public partial class TokenBuilder
    {
        public static TokenBuilder Simple()
        {
            var builder = new TokenBuilder();
            builder
                .Faker.CustomInstantiator(f => new Token(
                    f.Random.Guid().ToString(),
                    TokenType.Refresh
                ))
                .Ignore(t => t.TokenHash)
                .Ignore(t => t.Type);
            return builder
                .WithExpires(DateTime.Now.AddDays(1))
                .WithRevoked(false)
                .WithUser(UserBuilder.DefaultCustomer().Build());
        }

        public static TokenBuilder Typical()
        {
            return Simple();
        }
    }
}
