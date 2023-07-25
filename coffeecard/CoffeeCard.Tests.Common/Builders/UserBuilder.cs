using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders
{
    public class UserBuilder : BaseBuilder<User>
    {
        public override UserBuilder Simple()
        {
			var programme = new ProgrammeBuilder().Simple().Build();
			Faker.RuleFor(u => u.Id, f => f.IndexGlobal)
			.RuleFor(u => u.ProgrammeId, programme.Id)
			.RuleFor(u => u.Programme, programme)
			.RuleFor(u => u.Purchases, new List<Purchase>())
			.RuleFor(u => u.Statistics, new List<Statistic>())
			.RuleFor(u => u.LoginAttempts, new List<LoginAttempt>())
			.RuleFor(u => u.Tokens, new List<Token>())
			.RuleFor(u => u.UserState, UserState.Active)
			.RuleFor(u => u.IsVerified, true)
			.RuleFor(u => u.Tickets, new List<Ticket>());
			return this;
        }

		public UserBuilder DefaultCustomer(){
			Faker.RuleFor(u => u.UserGroup, UserGroup.Customer);
			return Simple();
		}

        public override UserBuilder Typical()
        {
            return Simple();
        }
    }
}