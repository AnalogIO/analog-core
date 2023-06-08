using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.DataTransferObjects.v2.User;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services.v2
{
    public class AccountServiceTest
    {

        private static User testuser => new User(
            email: "email",
            name: "name",
            password: "password",
            salt: "salt",
            programme: new Programme(fullName: "fullName", shortName: "shortName") { Id = 1 }
        );

        private CoffeeCardContext CreateTestCoffeeCardContextWithName(string name)
        {
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(name);

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
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
            var expected = testuser;
            expected.Name = "User";
            expected.Email = "test@test.test";
            User result;

            using var context = CreateTestCoffeeCardContextWithName(nameof(GetAccountByClaimsReturnsUserClaimWithEmail));
            context.Users.Add(expected);
            await context.SaveChangesAsync();
            // Act
            var accountService = new Library.Services.v2.AccountService(context, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, new Mock<IHashService>().Object);
            result = await accountService.GetAccountByClaimsAsync(claims);

            // Assert
            Assert.Equal(expected, result);
        }


        [Theory(DisplayName = "GetAccountByClaims throws ApiException, given invalid claim")]
        [MemberData(nameof(ClaimGenerator))]
        public async Task GetAccountByClaimsThrowsApiExceptionGivenInvalidClaim(IEnumerable<Claim> claims)
        {
            // Arrange
            var validUser = testuser;

            using var context = CreateTestCoffeeCardContextWithName(nameof(GetAccountByClaimsThrowsApiExceptionGivenInvalidClaim) + claims.ToString());
            if (!context.Users.Any())
            {
                // Avoid adding duplicate users
                context.Users.Add(validUser);
                await context.SaveChangesAsync();
            }

            // Act
            var accountService = new Library.Services.v2.AccountService(context, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, new Mock<IHashService>().Object);

            // Assert
            await Assert.ThrowsAsync<ApiException>(async () => await accountService.GetAccountByClaimsAsync(claims));
        }

        [Theory(DisplayName = "RegisterAccount returns user on valid input")]
        [InlineData("test", "test@test.test", "testPass", 1)]
        [InlineData("test1", "test", "1", 2)]
        [InlineData("test2", "test@test", "440", 3)]
        [InlineData("test3", "test+1@test.test", "Pass", 4)]
        public async Task RegisterAccountReturnsUserOnValidInput(String name, String email, string password, int programmeId)
        {
            // Arrange
            var programme = testuser.Programme;
            programme.Id = 1;
            var programme2 = testuser.Programme;
            programme2.Id = 2;
            var programme3 = testuser.Programme;
            programme3.Id = 3;
            var programme4 = testuser.Programme;
            programme4.Id = 4;

            var expected = testuser;
            expected.Name = name;
            expected.Email = email;
            expected.Password = "hashedPassword" + password;
            expected.Programme = programmeId switch
            {
                1 => programme,
                2 => programme2,
                3 => programme3,
                4 => programme4,
                _ => throw new Exception("Invalid programmeId")
            };
            expected.ProgrammeId = expected.Programme.Id;

            User result;

            // Using same context across all valid users to test creation of multiple users
            using var context = CreateTestCoffeeCardContextWithName(nameof(RegisterAccountReturnsUserOnValidInput));
            var emailServiceMock = new Mock<IEmailService>();
            emailServiceMock.Setup(e => e.SendRegistrationVerificationEmailAsync(It.IsAny<User>(), It.IsAny<String>())).Returns(Task.CompletedTask);
            var emailService = emailServiceMock.Object;

            var hashServiceMock = new Mock<IHashService>();
            hashServiceMock.Setup(h => h.GenerateSalt()).Returns("salt");
            hashServiceMock.Setup(h => h.Hash(It.IsAny<String>())).Returns("hashedPassword" + password);
            var hashService = hashServiceMock.Object;

            // Avoid adding duplicate programmes
            if (!context.Programmes.Any())
            {
                context.Programmes.Add(programme);
                context.Programmes.Add(programme2);
                context.Programmes.Add(programme3);
                context.Programmes.Add(programme4);
            }

            await context.SaveChangesAsync();
            // Act
            var accountService = new Library.Services.v2.AccountService(context, new Mock<ITokenService>().Object,
                emailService, hashService);
            result = await accountService.RegisterAccountAsync(name, email, password, programmeId);

            // Assert
            // Comparing specific properties instead of object since DateCreated would not be equals
            Assert.Equal(expected.Name, result.Name);
            Assert.Equal(expected.Programme.Id, result.Programme.Id);
            Assert.Equal(expected.Email, result.Email);
            Assert.Equal(expected.Password, result.Password);
        }

        [Fact(DisplayName = "RegisterAccount throws ApiException with status 409 on existing email")]
        public async Task RegisterAccountThrowsApiExceptionWithStatus409OnExistingEmail()
        {
            // Arrange
            var programme = new Programme(fullName: "fullName", shortName: "shortName");

            var user = testuser;
            var expectedSalt = "salt";

            using var context = CreateTestCoffeeCardContextWithName(nameof(RegisterAccountThrowsApiExceptionWithStatus409OnExistingEmail));
            context.Programmes.Add(programme);
            await context.SaveChangesAsync();

            var hashservice = new Mock<IHashService>();
            hashservice.Setup(h => h.GenerateSalt()).Returns(expectedSalt);
            hashservice.Setup(h => h.Hash(testuser.Password + expectedSalt)).Returns("hashedPassword");

            // Act
            var accountService = new Library.Services.v2.AccountService(context, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, hashservice.Object);

            // Assert
            // Register the first user
            await accountService.RegisterAccountAsync(user.Name, user.Email, user.Password, 1);
            // Try to register user with the sme email as before
            var exception = await Assert.ThrowsAsync<ApiException>(
                async () => await accountService.RegisterAccountAsync(user.Name, user.Email, user.Password, 1));
            Assert.Equal(StatusCodes.Status409Conflict, exception.StatusCode);
        }

        [Fact(DisplayName = "RegisterAccount throws ApiException with status 400 when given invalid programmeId")]
        public async Task RegisterAccountThrowsApiExceptionWithStatus400WhenGivenInvalidProgrammeId()
        {
            // Arrange
            using var context = CreateTestCoffeeCardContextWithName(nameof(RegisterAccountThrowsApiExceptionWithStatus400WhenGivenInvalidProgrammeId));
            // Act
            var accountService = new Library.Services.v2.AccountService(context, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, new Mock<IHashService>().Object);

            // Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                async () => await accountService.RegisterAccountAsync("name", "email", "pass", 1));
            Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        }

        [Fact(DisplayName = "RegisterAccount sends verification email only valid input")]
        public async Task RegisterAccountSendsVerificationEmailOnlyValidInput()
        {
            // Arrange
            var programme = new Programme(fullName: "fullName", shortName: "shortName");

            var expectedPassword = "password";

            var user = testuser;
            user.Password = expectedPassword;

            using var context = CreateTestCoffeeCardContextWithName(nameof(RegisterAccountSendsVerificationEmailOnlyValidInput));
            var emailServiceMock = new Mock<IEmailService>();
            emailServiceMock.Setup(e => e.SendRegistrationVerificationEmailAsync(It.IsAny<User>(), It.IsAny<String>())).Returns(Task.CompletedTask);
            var emailService = emailServiceMock.Object;

            var expectedSalt = "salt";
            var expectedSaltedPassword = "HashedPassword";
            var hashServiceMock = new Mock<IHashService>();
            hashServiceMock.Setup(h => h.GenerateSalt()).Returns(expectedSalt);
            hashServiceMock
                .Setup(h => h.Hash(expectedPassword + expectedSalt))
                .Returns(expectedSaltedPassword);
            var hashService = hashServiceMock.Object;

            context.Programmes.Add(programme);

            await context.SaveChangesAsync();
            // Act
            var accountService = new Library.Services.v2.AccountService(context, new Mock<ITokenService>().Object,
                emailService, hashService);
            await accountService.RegisterAccountAsync(user.Name, user.Email, user.Password, 1);

            // Assert
            // Verify an email would have been send for the first registration
            emailServiceMock.Verify(e => e.SendRegistrationVerificationEmailAsync(It.IsAny<User>(), It.IsAny<String>()), Times.Exactly(1));

            await Assert.ThrowsAsync<ApiException>(
                async () => await accountService.RegisterAccountAsync("name", "email", "pass", 1));
            // Verify no email was send for the second, failed registration
            emailServiceMock.Verify(e => e.SendRegistrationVerificationEmailAsync(It.IsAny<User>(), It.IsAny<String>()), Times.Exactly(1));
        }

        [Theory(DisplayName = "UpdateAccount updates all non null properties")]
        [InlineData("test1", "test@test.com", "password", false, 1)]
        [InlineData("test2", "test@test.com", "", false, null)]
        [InlineData("test3", "test@test.com", "pas", null, 1)]
        [InlineData("test4", "test@test.com", null, true, 1)]
        [InlineData("test5", "test@test.com", null, null, null)]
        public async Task UpdateAccountUpdatesAllNonNullProperties(String name, String email, String? password, bool? privacyActivated, int? programmeId)
        {
            // Arrange
            var updateUserRequest = new UpdateUserRequest()
            {
                Name = name,
                Email = email,
                Password = password,
                PrivacyActivated = privacyActivated,
                ProgrammeId = programmeId
            };
            var user = testuser;
            user.ProgrammeId = -1;

            var expected = testuser;
            expected.Name = name ?? user.Name;
            expected.Email = email ?? user.Email;
            expected.Password = password ?? user.Password;
            expected.PrivacyActivated = privacyActivated ?? user.PrivacyActivated;
            expected.ProgrammeId = programmeId ?? user.ProgrammeId;

            using var context = CreateTestCoffeeCardContextWithName(nameof(UpdateAccountUpdatesAllNonNullProperties) + name);
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var hashServiceMock = new Mock<IHashService>();
            hashServiceMock.Setup(h => h.Hash(It.IsAny<String>())).Returns(password ?? user.Password);
            var hashService = hashServiceMock.Object;

            // Act
            var accountService = new Library.Services.v2.AccountService(context, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, hashService);
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
            var programme = new Programme(fullName: "fullName", shortName: "shortName");

            var updateUserRequest = new UpdateUserRequest()
            {
                Name = "name",
                Email = "email",
                Password = "password",
                PrivacyActivated = false,
                ProgrammeId = 2 // No prgramme with Id
            };
            var user = testuser;
            user.Programme = programme;

            using var context = CreateTestCoffeeCardContextWithName(nameof(UpdateAccountThrowsApiExceptionOnInvalidProgrammeId));
            context.Users.Add(user);

            context.Programmes.Add(programme);
            await context.SaveChangesAsync();

            // Act
            var accountService = new Library.Services.v2.AccountService(context, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, new Mock<IHashService>().Object);

            // Assert
            var exception = await Assert.ThrowsAsync<ApiException>(async () => await accountService.UpdateAccountAsync(user, updateUserRequest));
            Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        }

        [Fact(DisplayName = "RequestAnonymization sends email")]
        public async Task RequestAnonymizationSendsEmail()
        {
            // Arrange
            var user = testuser;

            using var context = CreateTestCoffeeCardContextWithName(nameof(RequestAnonymizationSendsEmail));
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var emailServiceMock = new Mock<IEmailService>();
            emailServiceMock.Setup(e => e.SendVerificationEmailForDeleteAccount(It.IsAny<User>(), It.IsAny<String>())).Returns(Task.CompletedTask);
            var emailService = emailServiceMock.Object;

            // Act
            var accountService = new Library.Services.v2.AccountService(context, new Mock<ITokenService>().Object,
                emailService, new Mock<IHashService>().Object);

            await accountService.RequestAnonymizationAsync(user);
            // Assert
            emailServiceMock.Verify(e => e.SendVerificationEmailForDeleteAccount(It.IsAny<User>(), It.IsAny<String>()));
        }

        [Fact(DisplayName = "AnonymizeAccount removes identifyable information from user")]
        public async Task AnonymizeAccountRemovesIdentifyableInformationFromUser()
        {
            // Arrange
            var userEmail = "test@test.test";
            var user = testuser;
            user.Email = userEmail;

            var expected = testuser;
            expected.Id = 1;
            expected.Name = "";
            expected.Password = "";
            expected.Salt = "";
            expected.UserState = UserState.Deleted;
            expected.PrivacyActivated = true;
            expected.Email = "";

            await using var context = CreateTestCoffeeCardContextWithName(nameof(AnonymizeAccountRemovesIdentifyableInformationFromUser));
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var tokenServiceMock = new Mock<ITokenService>();
            tokenServiceMock.Setup(e => e.ValidateVerificationTokenAndGetEmail("test")).Returns(userEmail);

            // Act
            var accountService = new Library.Services.v2.AccountService(context, tokenServiceMock.Object,
                new Mock<IEmailService>().Object, new Mock<IHashService>().Object);

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

        public static IEnumerable<object[]> ClaimGenerator()
        {
            yield return new object[] {
                new List<Claim> // Good token, assuming user can be found in database 
                {
                    new Claim(ClaimTypes.Email, "userNotInDb@test.test"),
                    new Claim(ClaimTypes.Role, "")
                }
            };
            yield return new object[] {
                new List<Claim> // No email claim
                {
                    new Claim(ClaimTypes.Role, "verification_token")
                }
            };
        }
    }
}