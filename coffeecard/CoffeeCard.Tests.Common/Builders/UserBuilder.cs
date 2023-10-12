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
                // ID?
                //todo programme ID?
                .WithProgramme(programme)
                .WithPurchases(new List<Purchase>())
                .WithStatistics(new List<Statistic>())
                .WithLoginAttempts(new List<LoginAttempt>())
                .WithTokens(new List<Token>())
                .WithUserState(UserState.Active)
                .WithTickets(new List<Ticket>());
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