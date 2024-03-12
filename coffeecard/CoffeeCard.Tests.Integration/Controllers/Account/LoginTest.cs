using CoffeeCard.Models.DataTransferObjects.User;
using CoffeeCard.Tests.Common.Builders;
using CoffeeCard.Tests.Integration.WebApplication;
using CoffeeCard.WebApi;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CoffeeCard.Tests.Integration.Controllers.Account
{

    public class LoginTest : BaseIntegrationTest
    {
        private const string LoginUrl = "api/v1/account/login";
        public LoginTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Unknown_user_login_fails()
        {
            LoginDto loginRequest = new LoginDto
            {
                Password = "test",
                Email = "test@email.dk",
                Version = "2.1.0"
            };

            HttpResponseMessage response = await Client.PostAsJsonAsync(LoginUrl, loginRequest);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Known_user_login_succeeds_returns_token()
        {
            Models.Entities.User user = UserBuilder.DefaultCustomer().Build();
            string plaintextPassword = user.Password;
            user.Password = HashPassword(plaintextPassword + user.Salt);

            _ = await Context.Users.AddAsync(user);
            _ = await Context.SaveChangesAsync();

            LoginDto loginRequest = new LoginDto
            {
                Password = plaintextPassword,
                Email = user.Email,
                Version = "2.1.0"
            };
            HttpResponseMessage response = await Client.PostAsJsonAsync(LoginUrl, loginRequest);

            TokenDto token = await DeserializeResponseAsync<TokenDto>(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(token.Token!);
        }

        private static string HashPassword(string password)
        {
            byte[] byteArr = Encoding.UTF8.GetBytes(password);
            using SHA256 hasher = SHA256.Create();
            byte[] hashBytes = hasher.ComputeHash(byteArr);
            return Convert.ToBase64String(hashBytes);
        }
    }
}