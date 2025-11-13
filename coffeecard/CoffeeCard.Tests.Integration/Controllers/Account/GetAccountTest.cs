using System.Threading.Tasks;
using CoffeeCard.Models.Entities;
using CoffeeCard.Tests.Integration.WebApplication;
using CoffeeCard.WebApi;
using Xunit;

namespace CoffeeCard.Tests.Integration.Controllers.Account
{
    public class GetAccountTest(CustomWebApplicationFactory<Startup> factory)
        : BaseIntegrationTest(factory)
    {
        [Fact]
        public async Task Get_account_succeeds_when_authenticated_for_existing_account()
        {
            var user = await GetAuthenticatedUserAsync();

            var account = await CoffeeCardClientV2.Account_GetAsync();

            Assert.Equal(user.Email, account.Email);
            Assert.Equal(user.Name, account.Name);
            Assert.Equal(user.Programme.FullName, account.Programme.FullName);
            Assert.Equal(user.UserGroup.toUserRole().ToString(), account.Role.ToString());
        }
    }
}
