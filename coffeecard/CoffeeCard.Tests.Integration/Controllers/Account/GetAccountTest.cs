using System.Threading.Tasks;
using CoffeeCard.Tests.Integration.WebApplication;
using CoffeeCard.WebApi;
using Xunit;
using CoffeeCard.Models.DataTransferObjects.v2.User;
using CoffeeCard.Tests.Common.Builders;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Integration.Controllers.Account
{

    public class GetAccountTest : BaseIntegrationTest
    {
        private const string GetAccountUrl = "api/v2/account";
        public GetAccountTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Get_account_succeeds_when_authenticated_for_existing_account()
        {
            var user = UserBuilder.DefaultCustomer().Build();
            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();
            SetDefaultAuthHeader(user);

            var response = await Client.GetAsync(GetAccountUrl);
            var account = await DeserializeResponseAsync<UserResponse>(response);

            Assert.Equal(user.Email, account.Email);
            Assert.Equal(user.Name, account.Name);
            Assert.Equal(user.Programme.FullName, account.Programme.FullName);
            Assert.Equal(user.UserGroup.toUserRole(), account.Role);
        }

    }
}