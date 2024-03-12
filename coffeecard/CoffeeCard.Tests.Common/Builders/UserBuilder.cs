using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    [BuilderFor(typeof(User))]
    public partial class UserBuilder
    {
        public static UserBuilder Simple()
        {
            Programme programme = ProgrammeBuilder.Simple().Build();

            return new UserBuilder()
                .WithProgramme(programme)
                .WithPurchases([])
                .WithStatistics([])
                .WithLoginAttempts([])
                .WithTokens([])
                .WithUserState(UserState.Active)
                .WithTickets([]);
        }

        public static UserBuilder DefaultCustomer()
        {
            return Simple()
                .WithUserGroup(UserGroup.Customer)
                .WithIsVerified(true);
        }

        public static UserBuilder Typical()
        {
            return Simple();
        }
    }
}