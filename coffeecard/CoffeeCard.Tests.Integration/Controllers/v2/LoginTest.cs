using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
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
        public async Task Unknown_app_login_sends_welcome_email_without_persisting_registration_state()
        {
            Mock<IEmailSender> emailSenderMock = new Mock<IEmailSender>();
            MimeMessage sentMessage = null;
            emailSenderMock
                .Setup(x => x.SendEmailAsync(It.IsAny<MimeMessage>()))
                .Callback<MimeMessage>(message => sentMessage = message)
                .Returns(Task.CompletedTask);
            ConfigureMockService<IEmailSender>(emailSenderMock.Object);

            const string email = "new-user@email.dk";
            var loginRequest = new UserLoginRequest { Email = email, LoginType = LoginType.App };

            await CoffeeCardClientV2.Account_LoginAsync(loginRequest);

            Assert.Empty(await Context.Tokens.ToListAsync());
            Assert.NotNull(sentMessage);
            Assert.Contains("Welcome", sentMessage.Subject);
        }

        [Fact]
        public async Task Registration_token_is_not_accepted_as_an_api_access_token()
        {
            var registrationToken = Context
                .GetService<ITokenService>()
                .GenerateRegistrationToken("new-user@email.dk");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                registrationToken
            );

            var response = await HttpClient.GetAsync("/api/v2/account");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Complete_registration_creates_verified_user_and_returns_tokens()
        {
            var programme = ProgrammeBuilder.Simple().WithId(1).Build();
            await Context.Programmes.AddAsync(programme);
            await Context.SaveChangesAsync();

            const string email = "new-user@email.dk";
            var registrationToken = Context
                .GetService<ITokenService>()
                .GenerateRegistrationToken(email);

            var response = await CoffeeCardClientV2.Account_CompleteRegistrationAsync(
                new CompleteRegistrationRequest
                {
                    Token = registrationToken,
                    Name = "New User",
                    ProgrammeId = 1,
                }
            );

            Assert.NotNull(response.Jwt);
            Assert.NotNull(response.RefreshToken);
            Assert.True(new JwtSecurityTokenHandler().CanReadToken(response.Jwt));

            var user = await Context.Users.AsNoTracking().SingleAsync(u => u.Email == email);
            Assert.Equal("New User", user.Name);
            Assert.True(user.IsVerified);

            var tokens = await Context.Tokens.AsNoTracking().ToListAsync();
            Assert.Single(tokens);
            Assert.Equal(TokenType.Refresh, tokens[0].Type);
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
