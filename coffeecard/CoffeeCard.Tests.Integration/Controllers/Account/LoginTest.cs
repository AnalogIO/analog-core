using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CoffeeCard.Tests.ApiClient.Generated;
using CoffeeCard.Tests.Common.Builders;
using CoffeeCard.Tests.Integration.WebApplication;
using CoffeeCard.WebApi;
using Xunit;

namespace CoffeeCard.Tests.Integration.Controllers.Account
{
    public class LoginTest(CustomWebApplicationFactory<Startup> factory)
        : BaseIntegrationTest(factory)
    {
        [Fact]
        public async Task Unknown_user_login_fails()
        {
            var loginRequest = new LoginDto
            {
                Password = "test",
                Email = "test@email.dk",
                Version = "2.1.0",
            };

            var apiException = await Assert.ThrowsAsync<ApiException>(() =>
                CoffeeCardClient.Account_LoginAsync(loginRequest)
            );
            Assert.Equal((int)HttpStatusCode.Unauthorized, apiException.StatusCode);
        }

        [Fact]
        public async Task Known_user_login_succeeds_returns_token()
        {
            var user = UserBuilder.DefaultCustomer().Build();
            var plaintextPassword = user.Password;
            user.Password = HashPassword(plaintextPassword + user.Salt);

            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();

            var loginRequest = new LoginDto
            {
                Password = plaintextPassword,
                Email = user.Email,
                Version = "2.1.0",
            };
            var response = await CoffeeCardClient.Account_LoginAsync(loginRequest);

            Assert.NotEmpty(response.Token);
            var tokenValidator = new JwtSecurityTokenHandler();
            Assert.True(tokenValidator.CanReadToken(response.Token));
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
