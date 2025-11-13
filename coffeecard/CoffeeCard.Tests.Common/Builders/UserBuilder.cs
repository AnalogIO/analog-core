using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    [BuilderFor(typeof(User))]
    public partial class UserBuilder
    {
        public static UserBuilder Simple()
        {
            var programme = ProgrammeBuilder.Simple().Build();

            return new UserBuilder()
                .WithName(f => f.Person.FullName)
                .WithEmail(f => f.Person.Email)
                .WithPassword(f => f.Random.Utf16String(10, 15))
                .WithSalt("")
                .WithIsVerified(false)
                .WithExperience(f => f.Random.Int(min: 0, max: 5000))
                .WithDateCreated(new DateTime(2000, 1, 1))
                .WithDateUpdated(new DateTime(2000, 1, 1, 20, 0, 0))
                .WithPrivacyActivated(f => f.Random.Bool())
                .WithProgramme(programme)
                .WithPurchases(new List<Purchase>())
                .WithStatistics(new List<Statistic>())
                .WithTokens(new List<Token>())
                .WithUserState(UserState.Active)
                .WithTickets(new List<Ticket>());
        }

        public static UserBuilder DefaultCustomer()
        {
            return Simple().WithUserGroup(UserGroup.Customer).WithIsVerified(true);
        }

        public static UserBuilder Typical()
        {
            return Simple();
        }
    }
}
