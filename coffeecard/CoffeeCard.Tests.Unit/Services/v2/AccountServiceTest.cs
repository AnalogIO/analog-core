using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.v2.Programme;
using CoffeeCard.Models.DataTransferObjects.v2.User;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services.v2
{
    public class AccountServiceTest
    {
        private CoffeeCardContext CreateTestCoffeeCardContextWithName(string name)
        {
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                name
            );

            var databaseSettings = new DatabaseSettings { SchemaName = "test" };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            return new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
        }

        [Fact(DisplayName = "GetAccountByClaims returns user, given claim with email")]
        public async Task GetAccountByClaimsReturnsUserClaimWithEmail()
        {
            // Arrange
            var claims = new List<Claim>() { new Claim(ClaimTypes.Email, "test@test.test") };
            var expected = new User() { Name = "User", Email = "test@test.test" };
            User result;

            using var context = CreateTestCoffeeCardContextWithName(
                nameof(GetAccountByClaimsReturnsUserClaimWithEmail)
            );
            context.Users.Add(expected);
            await context.SaveChangesAsync();
            // Act
            var accountService = new Library.Services.v2.AccountService(
                context,
                new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object,
                new Mock<IHashService>().Object
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
            var validUser = new User() { Name = "User", Email = "test@test.test" };

            using var context = CreateTestCoffeeCardContextWithName(
                nameof(GetAccountByClaimsThrowsApiExceptionGivenInvalidClaim) + claims.ToString()
            );
            context.Users.Add(validUser);
            await context.SaveChangesAsync();
            // Act
            var accountService = new Library.Services.v2.AccountService(
                context,
                new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object,
                new Mock<IHashService>().Object
            );

            // Assert
            await Assert.ThrowsAsync<ApiException>(
                async () => await accountService.GetAccountByClaimsAsync(claims)
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
            var programme = new Programme()
            {
                Id = programmeId,
                FullName = "test",
                ShortName = "t",
                Users = new List<User>()
            };
            var expectedPass = "HashedPassword";
            var expected = new User()
            {
                Name = name,
                Email = email,
                Password = expectedPass,
                PrivacyActivated = false,
                Programme = programme
            };
            User result;

            // Using same context across all valid users to test creation of multiple users
            using var context = CreateTestCoffeeCardContextWithName(
                nameof(RegisterAccountReturnsUserOnValidInput)
            );
            var emailServiceMock = new Mock<IEmailService>();
            emailServiceMock
                .Setup(
                    e =>
                        e.SendRegistrationVerificationEmailAsync(
                            It.IsAny<User>(),
                            It.IsAny<String>()
                        )
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
            var accountService = new Library.Services.v2.AccountService(
                context,
                new Mock<ITokenService>().Object,
                emailService,
                hashService
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
            var programme = new Programme()
            {
                Id = 1,
                FullName = "test",
                ShortName = "t",
                SortPriority = 1,
                Users = new List<User>()
            };
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
            var accountService = new Library.Services.v2.AccountService(
                context,
                new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object,
                hashservice.Object
            );

            // Assert
            // Register the first user
            await accountService.RegisterAccountAsync("name", email, "pass", 1);
            // Try to register user with the sme email as before
            var exception = await Assert.ThrowsAsync<ApiException>(
                async () => await accountService.RegisterAccountAsync("name", email, "pass", 1)
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
            var accountService = new Library.Services.v2.AccountService(
                context,
                new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object,
                new Mock<IHashService>().Object
            );

            // Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                async () => await accountService.RegisterAccountAsync("name", "email", "pass", 1)
            );
            Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        }

        [Fact(DisplayName = "RegisterAccount sends verification email only valid input")]
        public async Task RegisterAccountSendsVerificationEmailOnlyValidInput()
        {
            // Arrange
            var programme = new Programme()
            {
                Id = 1,
                FullName = "test",
                ShortName = "t",
                Users = new List<User>()
            };
            var expectedPass = "HashedPassword";
            var expected = new User()
            {
                Name = "name",
                Email = "email",
                Password = expectedPass,
                PrivacyActivated = false,
                Programme = programme
            };

            using var context = CreateTestCoffeeCardContextWithName(
                nameof(RegisterAccountSendsVerificationEmailOnlyValidInput)
            );
            var emailServiceMock = new Mock<IEmailService>();
            emailServiceMock
                .Setup(
                    e =>
                        e.SendRegistrationVerificationEmailAsync(
                            It.IsAny<User>(),
                            It.IsAny<String>()
                        )
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
            var accountService = new Library.Services.v2.AccountService(
                context,
                new Mock<ITokenService>().Object,
                emailService,
                hashService
            );
            await accountService.RegisterAccountAsync("name", "email", "password", 1);

            // Assert
            // Verify an email would have been send for the first registration
            emailServiceMock.Verify(
                e => e.SendRegistrationVerificationEmailAsync(It.IsAny<User>(), It.IsAny<String>()),
                Times.Exactly(1)
            );

            await Assert.ThrowsAsync<ApiException>(
                async () => await accountService.RegisterAccountAsync("name", "email", "pass", 1)
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
            var programme = new Programme()
            {
                Id = 1,
                FullName = "test",
                ShortName = "t",
                Users = new List<User>()
            };
            var updateUserRequest = new UpdateUserRequest()
            {
                Name = name,
                Email = email,
                Password = password,
                PrivacyActivated = privacyActivated,
                ProgrammeId = programmeId
            };
            var user = new User()
            {
                Name = "name",
                Password = "pass",
                PrivacyActivated = false,
                Email = "test@test.test",
                Programme = programme
            };
            var expected = new User()
            {
                Name = name,
                Email = email,
                Password = password ?? user.Password,
                PrivacyActivated = privacyActivated ?? user.PrivacyActivated,
                Programme = programme ?? user.Programme
            };

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
            var accountService = new Library.Services.v2.AccountService(
                context,
                new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object,
                hashService
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
            var programme = new Programme()
            {
                Id = 1,
                FullName = "test",
                ShortName = "t",
                Users = new List<User>()
            };
            var updateUserRequest = new UpdateUserRequest()
            {
                Name = "name",
                Email = "email",
                Password = "password",
                PrivacyActivated = false,
                ProgrammeId = 2 // No prgramme with Id
            };
            var user = new User()
            {
                Name = "name",
                Password = "pass",
                PrivacyActivated = false,
                Email = "test@test.test",
                Programme = programme
            };

            using var context = CreateTestCoffeeCardContextWithName(
                nameof(UpdateAccountThrowsApiExceptionOnInvalidProgrammeId)
            );
            context.Users.Add(user);

            context.Programmes.Add(programme);
            await context.SaveChangesAsync();

            // Act
            var accountService = new Library.Services.v2.AccountService(
                context,
                new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object,
                new Mock<IHashService>().Object
            );

            // Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                async () => await accountService.UpdateAccountAsync(user, updateUserRequest)
            );
            Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        }

        [Fact(DisplayName = "RequestAnonymization sends email")]
        public async Task RequestAnonymizationSendsEmail()
        {
            // Arrange
            var user = new User()
            {
                Name = "name",
                Password = "pass",
                PrivacyActivated = false,
                Email = "test@test.test",
            };

            using var context = CreateTestCoffeeCardContextWithName(
                nameof(RequestAnonymizationSendsEmail)
            );
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var emailServiceMock = new Mock<IEmailService>();
            emailServiceMock
                .Setup(
                    e =>
                        e.SendVerificationEmailForDeleteAccount(
                            It.IsAny<User>(),
                            It.IsAny<String>()
                        )
                )
                .Returns(Task.CompletedTask);
            var emailService = emailServiceMock.Object;

            // Act
            var accountService = new Library.Services.v2.AccountService(
                context,
                new Mock<ITokenService>().Object,
                emailService,
                new Mock<IHashService>().Object
            );

            await accountService.RequestAnonymizationAsync(user);
            // Assert
            emailServiceMock.Verify(
                e => e.SendVerificationEmailForDeleteAccount(It.IsAny<User>(), It.IsAny<String>())
            );
        }

        [Fact(DisplayName = "AnonymizeAccount removes identifyable information from user")]
        public async Task AnonymizeAccountRemovesIdentifyableInformationFromUser()
        {
            // Arrange
            var userEmail = "test@test.test";
            var user = new User
            {
                Id = 1,
                Name = "name",
                Password = "pass",
                Salt = "salt",
                UserState = UserState.Active,
                PrivacyActivated = false,
                Email = userEmail,
            };
            var expected = new User
            {
                Id = 1,
                Name = "",
                Password = "",
                Salt = "",
                UserState = UserState.Deleted,
                PrivacyActivated = true,
                Email = "",
            };

            await using var context = CreateTestCoffeeCardContextWithName(
                nameof(AnonymizeAccountRemovesIdentifyableInformationFromUser)
            );
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var tokenServiceMock = new Mock<ITokenService>();
            tokenServiceMock
                .Setup(e => e.ValidateVerificationTokenAndGetEmail("test"))
                .Returns(userEmail);

            // Act
            var accountService = new Library.Services.v2.AccountService(
                context,
                tokenServiceMock.Object,
                new Mock<IEmailService>().Object,
                new Mock<IHashService>().Object
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
            var user = new User
            {
                Id = 1,
                Name = "name",
                Password = "pass",
                Salt = "salt",
                UserState = UserState.Active,
                PrivacyActivated = false,
                Email = userEmail,
                IsVerified = false
            };

            await using var context = CreateTestCoffeeCardContextWithName(
                nameof(ResendVerificationEmailWhenAccountIsNotVerified)
            );
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var emailService = new Mock<IEmailService>();
            var accountService = new Library.Services.v2.AccountService(
                context,
                new Mock<ITokenService>().Object,
                emailService.Object,
                new Mock<IHashService>().Object
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
            var user = new User
            {
                Id = 1,
                Name = "name",
                Password = "pass",
                Salt = "salt",
                UserState = UserState.Active,
                PrivacyActivated = false,
                Email = userEmail,
                IsVerified = true
            };

            await using var context = CreateTestCoffeeCardContextWithName(
                nameof(ResendVerificationEmailThrowsConflictExceptionWhenAccountIsAlreadyVerified)
            );
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var accountService = new Library.Services.v2.AccountService(
                context,
                new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object,
                new Mock<IHashService>().Object
            );

            await Assert.ThrowsAsync<ConflictException>(
                async () =>
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
            var accountService = new Library.Services.v2.AccountService(
                context,
                new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object,
                new Mock<IHashService>().Object
            );

            await Assert.ThrowsAsync<EntityNotFoundException>(
                async () =>
                    await accountService.ResendAccountVerificationEmail(
                        new ResendAccountVerificationEmailRequest { Email = "test@test.test" }
                    )
            );
        }

        public static IEnumerable<object[]> ClaimGenerator()
        {
            yield return new object[]
            {
                new List<Claim> // Good token, assuming user can be found in database
                {
                    new Claim(ClaimTypes.Email, "userNotInDb@test.test"),
                    new Claim(ClaimTypes.Role, "")
                }
            };
            yield return new object[]
            {
                new List<Claim> // No email claim
                {
                    new Claim(ClaimTypes.Role, "verification_token")
                }
            };
        }
    }
}
