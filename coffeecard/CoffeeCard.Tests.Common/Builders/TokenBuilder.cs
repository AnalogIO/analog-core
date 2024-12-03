using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    [BuilderFor(typeof(Token))]
    public partial class TokenBuilder
    {
        public static TokenBuilder Simple()
        {
            var builder = new TokenBuilder();
            builder.Faker.CustomInstantiator(f =>
                    new Token("tokenHash", TokenType.Refresh))
                    .RuleFor(o => o.TokenHash, f => f.Random.Guid().ToString())
                    .RuleFor(o => o.Type, f => TokenType.Refresh);
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