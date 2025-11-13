using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.Entities;
using CoffeeCard.Tests.ApiClient.v2.Generated;
using CoffeeCard.Tests.Common.Builders;
using CoffeeCard.Tests.Integration.WebApplication;
using CoffeeCard.WebApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using Moq;
using Xunit;
using LoginType = CoffeeCard.Tests.ApiClient.v2.Generated.LoginType;

namespace CoffeeCard.Tests.Integration.Controllers.v2.Account
{
    public class LoginTest(CustomWebApplicationFactory<Startup> factory)
        : BaseIntegrationTest(factory)
    {
        [Fact]
        public async Task Unknown_user_login_doesnt_fail_but_no_token_is_created()
        {
            // It should not be possible to determine if account with email exists, thus never fail
            var loginRequest = new UserLoginRequest
            {
                Email = "test@email.dk",
                LoginType = LoginType.Shifty,
            };

            var exception = await Record.ExceptionAsync(async () =>
                await CoffeeCardClientV2.Account_LoginAsync(loginRequest)
            );
            Assert.Null(exception);
            Assert.Empty(Context.Tokens);
        }

        [Fact]
        public async Task Known_user_login_saves_token_in_database_and_sends_one_mail()
        {
            Mock<IEmailSender> emailSenderMock = new Mock<IEmailSender>();
            ConfigureMockService<IEmailSender>(emailSenderMock.Object);

            var user = UserBuilder.DefaultCustomer().Build();

            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();

            var loginRequest = new UserLoginRequest
            {
                Email = user.Email,
                LoginType = LoginType.Shifty,
            };

            await CoffeeCardClientV2.Account_LoginAsync(loginRequest);

            var token = await Context.Tokens.FirstOrDefaultAsync();

            Assert.NotNull(token);
            Assert.Equal(TokenType.MagicLink, token.Type);
            Assert.Equal(user.Id, token.UserId);

            emailSenderMock.Verify(x => x.SendEmailAsync(It.IsAny<MimeMessage>()), Times.Once);
        }

        [Fact]
        public async Task Known_user_login_succeeds_returns_token()
        {
            var user = UserBuilder.DefaultCustomer().Build();
            var token = TokenBuilder.Simple().WithUser(user).WithType(TokenType.MagicLink).Build();
            var tokenString = token.TokenHash;
            var tokenHash = Context.GetService<IHashService>().Hash(token.TokenHash);
            token.TokenHash = tokenHash; // We need to hash the token before adding it to the database to ensure we don't leak credentials if database is breached

            await Context.Users.AddAsync(user);
            await Context.Tokens.AddAsync(token);
            await Context.SaveChangesAsync();

            // We authenticate using the non-hashed token and let the backend hash the string for us
            var response = await CoffeeCardClientV2.Account_AuthenticateAsync(
                new TokenLoginRequest() { Token = tokenString }
            );

            Assert.NotNull(response.Jwt);
            Assert.NotNull(response.RefreshToken);
            var tokenValidator = new JwtSecurityTokenHandler();
            Assert.True(tokenValidator.CanReadToken(response.Jwt));
        }
    }
}
