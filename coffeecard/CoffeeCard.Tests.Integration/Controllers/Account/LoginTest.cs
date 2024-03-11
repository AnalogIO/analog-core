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
            var loginRequest = new LoginDto
            {
                Password = "test",
                Email = "test@email.dk",
                Version = "2.1.0"
            };

            var response = await Client.PostAsJsonAsync(LoginUrl, loginRequest);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Known_user_login_succeeds_returns_token()
        {
            var user = UserBuilder.DefaultCustomer().Build();
            var plaintextPassword = user.Password;
            user.Password = HashPassword(plaintextPassword + user.Salt);

            _ = await Context.Users.AddAsync(user);
            _ = await Context.SaveChangesAsync();

            var loginRequest = new LoginDto
            {
                Password = plaintextPassword,
                Email = user.Email,
                Version = "2.1.0"
            };
            var response = await Client.PostAsJsonAsync(LoginUrl, loginRequest);

            var token = await DeserializeResponseAsync<TokenDto>(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(token.Token!);
        }

        private static string HashPassword(string password)
        {
            var byteArr = Encoding.UTF8.GetBytes(password);
            using var hasher = SHA256.Create();
            var hashBytes = hasher.ComputeHash(byteArr);
            return Convert.ToBase64String(hashBytes);
        }
    }
}