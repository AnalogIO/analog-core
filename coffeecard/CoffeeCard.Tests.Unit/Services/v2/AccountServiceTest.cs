using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.DataTransferObjects.v2.Token;
using CoffeeCard.Models.DataTransferObjects.v2.User;
using CoffeeCard.Models.Entities;
using CoffeeCard.Tests.Common.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using AccountService = CoffeeCard.Library.Services.v2.AccountService;

namespace CoffeeCard.Tests.Unit.Services.v2
{
    public class AccountServiceTest : BaseUnitTests
    {
        private CoffeeCardContext CreateTestCoffeeCardContextWithName(string name)
        {
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                name
            );

            var databaseSettings = new DatabaseSettings { SchemaName = "test" };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test,
            };

            return new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
        }

        [Fact(DisplayName = "GetAccountByClaims returns user, given claim with email")]
        public async Task GetAccountByClaimsReturnsUserClaimWithEmail()
        {
            // Arrange
            const string email = "test@test.test";
            var claims = new List<Claim>() { new Claim(ClaimTypes.Email, email) };
            var expected = UserBuilder.DefaultCustomer().WithEmail(email).Build();
            User result;

            using var context = CreateTestCoffeeCardContextWithName(
                nameof(GetAccountByClaimsReturnsUserClaimWithEmail)
            );
            context.Users.Add(expected);
            await context.SaveChangesAsync();
            // Act
            var accountService = new AccountService(
                context,
                new Mock<Library.Services.ITokenService>().Object,
                new Mock<Library.Services.IEmailService>().Object,
                new Mock<Library.Services.v2.IEmailService>().Object,
                new Mock<Library.Services.v2.ITokenService>().Object,
                new Mock<IHashService>().Object,
                NullLogger<AccountService>.Instance
            );
            result = await accountService.GetAccountByClaimsAsync(claims);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory(DisplayName = "GetAccountByClaims throws ApieEception, given invalid claim")]
        [MemberData(nameof(ClaimGenerator))]
        public async Task GetAccountByClaimsThrowsApiExceptionGivenInvalidClaim(
            IEnumerable<Claim> claims
        )
        {
            // Arrange
            var validUser = UserBuilder.DefaultCustomer().Build();

            using var context = CreateTestCoffeeCardContextWithName(
                nameof(GetAccountByClaimsThrowsApiExceptionGivenInvalidClaim) + claims
            );
            context.Users.Add(validUser);
            await context.SaveChangesAsync();
            // Act
            var accountService = new AccountService(
                context,
                new Mock<Library.Services.ITokenService>().Object,
                new Mock<Library.Services.IEmailService>().Object,
                new Mock<Library.Services.v2.IEmailService>().Object,
                new Mock<Library.Services.v2.ITokenService>().Object,
                new Mock<IHashService>().Object,
                NullLogger<AccountService>.Instance
            );

            // Assert
            await Assert.ThrowsAsync<ApiException>(async () =>
                await accountService.GetAccountByClaimsAsync(claims)
            );
        }

        [Theory(DisplayName = "RegisterAccount returns user on valid input")]
        [InlineData("test", "test@test.test", "testPass", 1)]
        [InlineData("test1", "test", "1", 2)]
        [InlineData("test2", "test@test", "440", 3)]
        [InlineData("test3", "test+1@test.test", "Pass", 4)]
        public async Task RegisterAccountReturnsUserOnValidInput(
            String name,
            String email,
            string password,
            int programmeId
        )
        {
            // Arrange
            var programme = ProgrammeBuilder.Simple().WithId(programmeId).Build();
            var expectedPass = "HashedPassword";
            var expected = UserBuilder
                .DefaultCustomer()
                .WithName(name)
                .WithEmail(email)
                .WithPassword(expectedPass)
                .WithProgramme(programme)
                .WithPrivacyActivated(false)
                .Build();
            User result;

            // Using same context across all valid users to test creation of multiple users
            using var context = CreateTestCoffeeCardContextWithName(
                nameof(RegisterAccountReturnsUserOnValidInput)
            );
            var emailServiceMock = new Mock<Library.Services.IEmailService>();
            emailServiceMock
                .Setup(e =>
                    e.SendRegistrationVerificationEmailAsync(It.IsAny<User>(), It.IsAny<String>())
                )
                .Returns(Task.CompletedTask);
            var emailService = emailServiceMock.Object;

            var hashServiceMock = new Mock<IHashService>();
            hashServiceMock.Setup(h => h.GenerateSalt()).Returns("");
            hashServiceMock.Setup(h => h.Hash(password)).Returns(expectedPass);
            var hashService = hashServiceMock.Object;

            context.Programmes.Add(programme);
            await context.SaveChangesAsync();
            // Act
            var accountService = new AccountService(
                context,
                new Mock<Library.Services.ITokenService>().Object,
                emailService,
                new Mock<Library.Services.v2.IEmailService>().Object,
                new Mock<Library.Services.v2.ITokenService>().Object,
                hashService,
                NullLogger<AccountService>.Instance
            );
            result = await accountService.RegisterAccountAsync(name, email, password, programmeId);

            // Assert
            // Comparing specific properties instead of object since DateCreated would not be equals
            Assert.Equal(expected.Name, result.Name);
            Assert.Equal(expected.Programme.Id, result.Programme.Id);
            Assert.Equal(expected.Email, result.Email);
            Assert.Equal(expected.Password, result.Password);
        }

        [Fact(
            DisplayName = "RegisterAccount throws ApiException with status 409 on existing email"
        )]
        public async Task RegisterAccountThrowsApiExceptionWithStatus409OnExistingEmail()
        {
            // Arrange
            var programme = ProgrammeBuilder.Simple().Build();
            var email = "test@test.dk";

            using var context = CreateTestCoffeeCardContextWithName(
                nameof(RegisterAccountThrowsApiExceptionWithStatus409OnExistingEmail)
            );
            context.Programmes.Add(programme);
            await context.SaveChangesAsync();

            var hashservice = new Mock<IHashService>();
            hashservice.Setup(h => h.GenerateSalt()).Returns("");
            hashservice.Setup(h => h.Hash("pass")).Returns("");

            // Act
            var accountService = new AccountService(
                context,
                new Mock<Library.Services.ITokenService>().Object,
                new Mock<Library.Services.IEmailService>().Object,
                new Mock<Library.Services.v2.IEmailService>().Object,
                new Mock<Library.Services.v2.ITokenService>().Object,
                hashservice.Object,
                NullLogger<AccountService>.Instance
            );
            // Assert
            // Register the first user
            await accountService.RegisterAccountAsync("name", email, "pass", 1);
            // Try to register user with the sme email as before
            var exception = await Assert.ThrowsAsync<ApiException>(async () =>
                await accountService.RegisterAccountAsync("name", email, "pass", 1)
            );
            Assert.Equal(StatusCodes.Status409Conflict, exception.StatusCode);
        }

        [Fact(
            DisplayName = "RegisterAccount throws ApiException with status 400 when given invalid programmeId"
        )]
        public async Task RegisterAccountThrowsApiExceptionWithStatus400WhenGivenInvalidProgrammeId()
        {
            // Arrange
            using var context = CreateTestCoffeeCardContextWithName(
                nameof(RegisterAccountThrowsApiExceptionWithStatus400WhenGivenInvalidProgrammeId)
            );
            // Act
            var accountService = new AccountService(
                context,
                new Mock<Library.Services.ITokenService>().Object,
                new Mock<Library.Services.IEmailService>().Object,
                new Mock<Library.Services.v2.IEmailService>().Object,
                new Mock<Library.Services.v2.ITokenService>().Object,
                new Mock<IHashService>().Object,
                NullLogger<AccountService>.Instance
            );

            // Assert
            var exception = await Assert.ThrowsAsync<ApiException>(async () =>
                await accountService.RegisterAccountAsync("name", "email", "pass", 1)
            );
            Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        }

        [Fact(DisplayName = "RegisterAccount sends verification email only valid input")]
        public async Task RegisterAccountSendsVerificationEmailOnlyValidInput()
        {
            // Arrange
            var programme = ProgrammeBuilder.Simple().Build();
            var expectedPass = "HashedPassword";
            var expected = UserBuilder
                .DefaultCustomer()
                .WithPassword(expectedPass)
                .WithProgramme(programme)
                .Build();

            using var context = CreateTestCoffeeCardContextWithName(
                nameof(RegisterAccountSendsVerificationEmailOnlyValidInput)
            );
            var emailServiceMock = new Mock<Library.Services.IEmailService>();
            emailServiceMock
                .Setup(e =>
                    e.SendRegistrationVerificationEmailAsync(It.IsAny<User>(), It.IsAny<String>())
                )
                .Returns(Task.CompletedTask);
            var emailService = emailServiceMock.Object;

            var hashServiceMock = new Mock<IHashService>();
            hashServiceMock.Setup(h => h.GenerateSalt()).Returns("");
            hashServiceMock.Setup(h => h.Hash("password")).Returns(expectedPass);
            var hashService = hashServiceMock.Object;

            context.Programmes.Add(programme);
            await context.SaveChangesAsync();
            // Act
            var accountService = new AccountService(
                context,
                new Mock<Library.Services.ITokenService>().Object,
                emailService,
                new Mock<Library.Services.v2.IEmailService>().Object,
                new Mock<Library.Services.v2.ITokenService>().Object,
                hashService,
                NullLogger<AccountService>.Instance
            );
            await accountService.RegisterAccountAsync("name", "email", "password", 1);

            // Assert
            // Verify an email would have been send for the first registration
            emailServiceMock.Verify(
                e => e.SendRegistrationVerificationEmailAsync(It.IsAny<User>(), It.IsAny<String>()),
                Times.Exactly(1)
            );

            await Assert.ThrowsAsync<ApiException>(async () =>
                await accountService.RegisterAccountAsync("name", "email", "pass", 1)
            );
            // Verify no email was send for the second, failed registration
            emailServiceMock.Verify(
                e => e.SendRegistrationVerificationEmailAsync(It.IsAny<User>(), It.IsAny<String>()),
                Times.Exactly(1)
            );
        }

        [Theory(DisplayName = "UpdateAccount updates all non null properties")]
        [InlineData("test1", "test@test.com", "password", false, 1)]
        [InlineData("test2", "test@test.com", "", false, null)]
        [InlineData("test3", "test@test.com", "pas", null, 1)]
        [InlineData("test4", "test@test.com", null, true, 1)]
        [InlineData("test5", "test@test.com", null, null, null)]
        public async Task UpdateAccountUpdatesAllNonNullProperties(
            String name,
            String email,
            String? password,
            bool? privacyActivated,
            int? programmeId
        )
        {
            // Arrange
            var programme = ProgrammeBuilder.Simple().Build();
            var updateUserRequest = new UpdateUserRequest()
            {
                Name = name,
                Email = email,
                Password = password,
                PrivacyActivated = privacyActivated,
                ProgrammeId = programmeId,
            };
            var user = UserBuilder.DefaultCustomer().WithProgramme(programme).Build();
            var expected = UserBuilder
                .DefaultCustomer()
                .WithName(name)
                .WithEmail(email)
                .WithPassword(password ?? user.Password)
                .WithPrivacyActivated(privacyActivated ?? user.PrivacyActivated)
                .WithProgramme(programme ?? user.Programme)
                .Build();

            using var context = CreateTestCoffeeCardContextWithName(
                nameof(UpdateAccountUpdatesAllNonNullProperties) + name
            );
            context.Users.Add(user);

            context.Programmes.Add(programme);
            await context.SaveChangesAsync();

            var hashServiceMock = new Mock<IHashService>();
            hashServiceMock.Setup(h => h.Hash(It.IsAny<String>())).Returns(password);
            var hashService = hashServiceMock.Object;

            // Act
            var accountService = new AccountService(
                context,
                new Mock<Library.Services.ITokenService>().Object,
                new Mock<Library.Services.IEmailService>().Object,
                new Mock<Library.Services.v2.IEmailService>().Object,
                new Mock<Library.Services.v2.ITokenService>().Object,
                hashService,
                NullLogger<AccountService>.Instance
            );
            var result = await accountService.UpdateAccountAsync(user, updateUserRequest);

            // Assert
            Assert.Equal(expected.Name, result.Name);
            Assert.Equal(expected.Programme.Id, result.Programme.Id);
            Assert.Equal(expected.PrivacyActivated, result.PrivacyActivated);
            Assert.Equal(expected.Email, result.Email);
            Assert.Equal(expected.Password, result.Password);
        }

        [Fact(DisplayName = "UpdateAccount throws ApiException on invalid programme id")]
        public async Task UpdateAccountThrowsApiExceptionOnInvalidProgrammeId()
        {
            // Arrange
            var programme = ProgrammeBuilder.Simple().WithId(1).Build();
            var updateUserRequest = new UpdateUserRequest()
            {
                Name = "name",
                Email = "email",
                Password = "password",
                PrivacyActivated = false,
                ProgrammeId = 2, // No prgramme with Id
            };
            var user = UserBuilder.DefaultCustomer().WithProgramme(programme).Build();

            using var context = CreateTestCoffeeCardContextWithName(
                nameof(UpdateAccountThrowsApiExceptionOnInvalidProgrammeId)
            );
            context.Users.Add(user);

            context.Programmes.Add(programme);
            await context.SaveChangesAsync();

            // Act
            var accountService = new AccountService(
                context,
                new Mock<Library.Services.ITokenService>().Object,
                new Mock<Library.Services.IEmailService>().Object,
                new Mock<Library.Services.v2.IEmailService>().Object,
                new Mock<Library.Services.v2.ITokenService>().Object,
                new Mock<IHashService>().Object,
                NullLogger<AccountService>.Instance
            );

            // Assert
            var exception = await Assert.ThrowsAsync<ApiException>(async () =>
                await accountService.UpdateAccountAsync(user, updateUserRequest)
            );
            Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        }

        [Fact(DisplayName = "RequestAnonymization sends email")]
        public async Task RequestAnonymizationSendsEmail()
        {
            // Arrange
            var user = UserBuilder.DefaultCustomer().Build();

            using var context = CreateTestCoffeeCardContextWithName(
                nameof(RequestAnonymizationSendsEmail)
            );
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var emailServiceMock = new Mock<Library.Services.IEmailService>();
            emailServiceMock
                .Setup(e =>
                    e.SendVerificationEmailForDeleteAccount(It.IsAny<User>(), It.IsAny<String>())
                )
                .Returns(Task.CompletedTask);
            var emailService = emailServiceMock.Object;

            // Act
            var accountService = new AccountService(
                context,
                new Mock<Library.Services.ITokenService>().Object,
                emailService,
                new Mock<Library.Services.v2.IEmailService>().Object,
                new Mock<Library.Services.v2.ITokenService>().Object,
                new Mock<IHashService>().Object,
                NullLogger<AccountService>.Instance
            );

            await accountService.RequestAnonymizationAsync(user);
            // Assert
            emailServiceMock.Verify(e =>
                e.SendVerificationEmailForDeleteAccount(It.IsAny<User>(), It.IsAny<String>())
            );
        }

        [Fact(DisplayName = "AnonymizeAccount removes identifyable information from user")]
        public async Task AnonymizeAccountRemovesIdentifyableInformationFromUser()
        {
            // Arrange
            var userEmail = "test@test.test";
            var user = UserBuilder
                .DefaultCustomer()
                .WithEmail(userEmail)
                .WithUserState(UserState.Active)
                .Build();
            var expected = UserBuilder
                .DefaultCustomer()
                .WithPrivacyActivated(user.PrivacyActivated)
                .WithName("")
                .WithPassword("")
                .WithSalt("")
                .WithEmail("")
                .WithUserState(UserState.Deleted)
                .Build();

            await using var context = CreateTestCoffeeCardContextWithName(
                nameof(AnonymizeAccountRemovesIdentifyableInformationFromUser)
            );
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var tokenServiceMock = new Mock<Library.Services.ITokenService>();
            tokenServiceMock
                .Setup(e => e.ValidateVerificationTokenAndGetEmail("test"))
                .Returns(userEmail);

            // Act
            var accountService = new AccountService(
                context,
                tokenServiceMock.Object,
                new Mock<Library.Services.IEmailService>().Object,
                new Mock<Library.Services.v2.IEmailService>().Object,
                new Mock<Library.Services.v2.ITokenService>().Object,
                new Mock<IHashService>().Object,
                NullLogger<AccountService>.Instance
            );

            await accountService.AnonymizeAccountAsync("test");
            var result = await context.Users.Where(u => u.Id == user.Id).FirstAsync();

            // Assert
            // Comparing specific properties instead of object since DateCreated would not be equals
            Assert.Equal(expected.Name, result.Name);
            Assert.Equal(expected.Salt, result.Salt);
            Assert.Equal(expected.PrivacyActivated, result.PrivacyActivated);
            Assert.Equal(expected.Email, result.Email);
            Assert.Equal(expected.Password, result.Password);
            Assert.Equal(expected.UserState, result.UserState);
        }

        [Fact(DisplayName = "Resend verification email when account is not already verified")]
        public async Task ResendVerificationEmailWhenAccountIsNotVerified()
        {
            // Arrange
            const string userEmail = "test@test.test";
            var user = UserBuilder
                .DefaultCustomer()
                .WithEmail(userEmail)
                .WithIsVerified(false)
                .Build();

            await using var context = CreateTestCoffeeCardContextWithName(
                nameof(ResendVerificationEmailWhenAccountIsNotVerified)
            );
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var emailService = new Mock<Library.Services.IEmailService>();
            var accountService = new AccountService(
                context,
                new Mock<Library.Services.ITokenService>().Object,
                emailService.Object,
                new Mock<Library.Services.v2.IEmailService>().Object,
                new Mock<Library.Services.v2.ITokenService>().Object,
                new Mock<IHashService>().Object,
                NullLogger<AccountService>.Instance
            );

            await accountService.ResendAccountVerificationEmail(
                new ResendAccountVerificationEmailRequest { Email = userEmail }
            );

            // Assert
            emailService.Verify(
                e => e.SendRegistrationVerificationEmailAsync(user, It.IsAny<string>()),
                Times.Once
            );
        }

        [Fact(
            DisplayName = "Resend verification email throws ConflictException when already verified"
        )]
        public async Task ResendVerificationEmailThrowsConflictExceptionWhenAccountIsAlreadyVerified()
        {
            // Arrange
            const string userEmail = "test@test.test";
            var user = UserBuilder
                .DefaultCustomer()
                .WithEmail(userEmail)
                .WithIsVerified(true)
                .Build();

            await using var context = CreateTestCoffeeCardContextWithName(
                nameof(ResendVerificationEmailThrowsConflictExceptionWhenAccountIsAlreadyVerified)
            );
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var accountService = new AccountService(
                context,
                new Mock<Library.Services.ITokenService>().Object,
                new Mock<Library.Services.IEmailService>().Object,
                new Mock<Library.Services.v2.IEmailService>().Object,
                new Mock<Library.Services.v2.ITokenService>().Object,
                new Mock<IHashService>().Object,
                NullLogger<AccountService>.Instance
            );

            await Assert.ThrowsAsync<ConflictException>(async () =>
                await accountService.ResendAccountVerificationEmail(
                    new ResendAccountVerificationEmailRequest { Email = userEmail }
                )
            );
        }

        [Fact(
            DisplayName = "Resend verification email throws EntityNotFoundException when email doesnot exist"
        )]
        public async Task ResendVerificationEmailThrowsEntityNotFoundExceptionWhenEmailDoesnotExist()
        {
            // Arrange
            await using var context = CreateTestCoffeeCardContextWithName(
                nameof(ResendVerificationEmailThrowsEntityNotFoundExceptionWhenEmailDoesnotExist)
            );

            // Act
            var accountService = new AccountService(
                context,
                new Mock<Library.Services.ITokenService>().Object,
                new Mock<Library.Services.IEmailService>().Object,
                new Mock<Library.Services.v2.IEmailService>().Object,
                new Mock<Library.Services.v2.ITokenService>().Object,
                new Mock<IHashService>().Object,
                NullLogger<AccountService>.Instance
            );

            await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                await accountService.ResendAccountVerificationEmail(
                    new ResendAccountVerificationEmailRequest { Email = "test@test.test" }
                )
            );
        }

        [Fact(DisplayName = "SendMagicLink sends email when user is found")]
        public async Task SendMagicLinkSendsEmailWhenUserIsFound()
        {
            // Arrange
            const string userEmail = "john@cena.com";
            var user = UserBuilder.DefaultCustomer().WithEmail(userEmail).Build();

            await using var context = CreateTestCoffeeCardContextWithName(
                nameof(SendMagicLinkSendsEmailWhenUserIsFound)
            );

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var emailService = new Mock<Library.Services.v2.IEmailService>();
            var accountService = new Library.Services.v2.AccountService(
                context,
                new Mock<Library.Services.ITokenService>().Object,
                new Mock<Library.Services.IEmailService>().Object,
                emailService.Object,
                new Mock<Library.Services.v2.ITokenService>().Object,
                new Mock<IHashService>().Object,
                NullLogger<AccountService>.Instance
            );

            // Act
            await accountService.SendMagicLinkEmail(userEmail, LoginType.Shifty);

            // Assert
            emailService.Verify(
                e => e.SendMagicLink(user, It.IsAny<string>(), It.IsAny<LoginType>()),
                Times.Once
            );
        }

        [Fact(DisplayName = "SendMagicLink does not send mail when user is not found")]
        public async Task SendMagicLinkDoesNotSendMailWhenUserIsNotFound()
        {
            // Arrange
            await using var context = CreateTestCoffeeCardContextWithName(
                nameof(SendMagicLinkDoesNotSendMailWhenUserIsNotFound)
            );

            var emailService = new Mock<Library.Services.v2.IEmailService>();
            var accountService = new Library.Services.v2.AccountService(
                context,
                new Mock<Library.Services.ITokenService>().Object,
                new Mock<Library.Services.IEmailService>().Object,
                emailService.Object,
                new Mock<Library.Services.v2.ITokenService>().Object,
                new Mock<IHashService>().Object,
                NullLogger<AccountService>.Instance
            );

            // Act
            await accountService.SendMagicLinkEmail("nonexisting@email.com", LoginType.Shifty);

            // Assert
            emailService.Verify(
                e => e.SendMagicLink(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<LoginType>()),
                Times.Never
            );
        }

        [Fact(DisplayName = "GenerateTokenPair revokes token on use")]
        public async Task GenerateTokenPairRevokesTokenOnUse()
        {
            // Arrange
            var user = UserBuilder.DefaultCustomer().Build();

            const string tokenHash = "refreshToken";

            var refreshToken = TokenBuilder
                .Simple()
                .WithTokenHash(tokenHash)
                .WithType(TokenType.Refresh)
                .WithUser(user)
                .Build();

            await using var context = CreateTestCoffeeCardContextWithName(
                nameof(GenerateTokenPairRevokesTokenOnUse)
            );
            await context.Users.AddAsync(user);
            await context.Tokens.AddAsync(refreshToken);
            await context.SaveChangesAsync();

            var tokenService = new Mock<Library.Services.v2.ITokenService>();
            tokenService
                .Setup(t => t.GetValidTokenByHashAsync("refreshToken"))
                .ReturnsAsync(refreshToken);

            var accountService = new Library.Services.v2.AccountService(
                context,
                new Mock<Library.Services.ITokenService>().Object,
                new Mock<Library.Services.IEmailService>().Object,
                new Mock<Library.Services.v2.IEmailService>().Object,
                tokenService.Object,
                new Mock<IHashService>().Object,
                NullLogger<AccountService>.Instance
            );

            // Act
            var tokenPair = await accountService.GenerateUserLoginFromToken(
                new TokenLoginRequest() { Token = tokenHash }
            );

            // Assert
            Assert.True(refreshToken.Revoked);
        }

        [Fact(DisplayName = "GenerateTokenPair returns token pair")]
        public async Task GenerateTokenPairReturnsTokenPair()
        {
            // Arrange
            var user = UserBuilder.DefaultCustomer().Build();

            const string tokenHash = "refreshToken";

            var refreshToken = TokenBuilder
                .Simple()
                .WithTokenHash(tokenHash)
                .WithType(TokenType.Refresh)
                .WithUser(user)
                .Build();

            await using var context = CreateTestCoffeeCardContextWithName(
                nameof(GenerateTokenPairReturnsTokenPair)
            );
            await context.Users.AddAsync(user);
            await context.Tokens.AddAsync(refreshToken);
            await context.SaveChangesAsync();

            var tokenServicev2 = new Mock<Library.Services.v2.ITokenService>();
            tokenServicev2.Setup(t => t.GenerateRefreshTokenAsync(user)).ReturnsAsync("newToken");
            tokenServicev2
                .Setup(t => t.GetValidTokenByHashAsync(tokenHash))
                .ReturnsAsync(refreshToken);

            var tokenServicev1 = new Mock<Library.Services.ITokenService>();
            tokenServicev1
                .Setup(t => t.GenerateToken(It.IsAny<IEnumerable<Claim>>()))
                .Returns("jwtToken");

            var accountService = new Library.Services.v2.AccountService(
                context,
                tokenServicev1.Object,
                new Mock<Library.Services.IEmailService>().Object,
                new Mock<Library.Services.v2.IEmailService>().Object,
                tokenServicev2.Object,
                new Mock<IHashService>().Object,
                NullLogger<AccountService>.Instance
            );

            // Act
            var tokenPair = await accountService.GenerateUserLoginFromToken(
                new TokenLoginRequest() { Token = tokenHash }
            );

            // Assert
            Assert.NotNull(tokenPair);
            Assert.NotNull(tokenPair.RefreshToken);
            Assert.NotNull(tokenPair.Jwt);
        }

        public static IEnumerable<object[]> ClaimGenerator()
        {
            yield return new object[]
            {
                new List<Claim> // Good token, assuming user can be found in database
                {
                    new Claim(ClaimTypes.Email, "userNotInDb@test.test"),
                    new Claim(ClaimTypes.Role, ""),
                },
            };
            yield return new object[]
            {
                new List<Claim> // No email claim
                {
                    new Claim(ClaimTypes.Role, "verification_token"),
                },
            };
        }
    }
}
