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
        private CoffeeCardContext CreateTestCoffeeCardContextWithName(String name)
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
            var expected = new User() { Email = "test@test.test" };
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


        [Theory(DisplayName = "GetAccountByClaims throws ApieEception, given invalid claim")]
        [MemberData(nameof(ClaimGenerator))]
        public async Task GetAccountByClaimsThrowsApiExceptionGivenInvalidClaim(IEnumerable<Claim> claims)
        {
            // Arrange
            var validUser = new User() { Email = "test@test.test" };

            using var context = CreateTestCoffeeCardContextWithName(nameof(GetAccountByClaimsThrowsApiExceptionGivenInvalidClaim) + claims.ToString());
            context.Users.Add(validUser);
            await context.SaveChangesAsync();
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
            using var context = CreateTestCoffeeCardContextWithName(nameof(RegisterAccountReturnsUserOnValidInput));
            var emailServiceMock = new Mock<IEmailService>();
            emailServiceMock.Setup(e => e.SendRegistrationVerificationEmailAsync(It.IsAny<User>(), It.IsAny<String>())).Returns(Task.CompletedTask);
            var emailService = emailServiceMock.Object;

            var hashServiceMock = new Mock<IHashService>();
            hashServiceMock.Setup(h => h.Hash(It.IsAny<String>())).Returns(expectedPass);
            var hashService = hashServiceMock.Object;

            context.Programmes.Add(programme);
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
            var programme = new Programme() { Id = 1, FullName = "test", ShortName = "t", SortPriority = 1, Users = new List<User>() };
            var email = "test@test.dk";

            using var context = CreateTestCoffeeCardContextWithName(nameof(RegisterAccountThrowsApiExceptionWithStatus409OnExistingEmail));
            context.Programmes.Add(programme);
            await context.SaveChangesAsync();
            // Act
            var accountService = new Library.Services.v2.AccountService(context, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, new Mock<IHashService>().Object);

            // Assert
            // Register the first user
            await accountService.RegisterAccountAsync("name", email, "pass", 1);
            // Try to register user with the sme email as before
            var exception = await Assert.ThrowsAsync<ApiException>(
                async () => await accountService.RegisterAccountAsync("name", email, "pass", 1));
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
            User result;

            // Using same context across all valid users to test creation of multiple users
            using var context = CreateTestCoffeeCardContextWithName(nameof(RegisterAccountSendsVerificationEmailOnlyValidInput));
            var emailServiceMock = new Mock<IEmailService>();
            emailServiceMock.Setup(e => e.SendRegistrationVerificationEmailAsync(It.IsAny<User>(), It.IsAny<String>())).Returns(Task.CompletedTask);
            var emailService = emailServiceMock.Object;

            var hashServiceMock = new Mock<IHashService>();
            hashServiceMock.Setup(h => h.Hash(It.IsAny<String>())).Returns(expectedPass);
            var hashService = hashServiceMock.Object;

            context.Programmes.Add(programme);
            await context.SaveChangesAsync();
            // Act
            var accountService = new Library.Services.v2.AccountService(context, new Mock<ITokenService>().Object,
                emailService, hashService);
            result = await accountService.RegisterAccountAsync("name", "email", "password", 1);

            // Assert
            // Verify an email would have been send for the first registration
            emailServiceMock.Verify(e => e.SendRegistrationVerificationEmailAsync(It.IsAny<User>(), It.IsAny<String>()), Times.Exactly(1));

            await Assert.ThrowsAsync<ApiException>(
                async () => await accountService.RegisterAccountAsync("name", "email", "pass", 1));
            // Verify no email was send for the second, failed registration
            emailServiceMock.Verify(e => e.SendRegistrationVerificationEmailAsync(It.IsAny<User>(), It.IsAny<String>()), Times.Exactly(1));
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