using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services
{
    public class TokenServiceTest
    {
        public TokenServiceTest()
        {
            _identity = new IdentitySettings();

            //creates the key for signing the token
            const string keyForHmacSha256 = "signingKey";
            _identity.TokenKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(keyForHmacSha256)).ToString();
        }

        private readonly IdentitySettings _identity;

        private static User testuser => new User(
            email: "test",
            name: "test",
            password: "pass",
            salt: "salt",
            programme: new Programme(fullName: "fullName", shortName: "shortName")
        );

        [Fact(DisplayName = "ValidateToken given invalid token returns false")]
        public async Task ValidateTokenGivenInvalidTokenReturnsFalse()
        {
            // Arrange
            bool result;

            var context = GenerateCoffeeCardContext(nameof(ValidateTokenGivenInvalidTokenReturnsFalse));
            await using (context)
            {
                var claimsUtility = new ClaimsUtilities(context);
                var tokenService = new TokenService(_identity, claimsUtility);

                // Act
                result = await tokenService.ValidateTokenIsUnusedAsync("Bogus token");
            }

            // Assert
            Assert.False(result);
        }

        [Fact(DisplayName = "ValidateToken given valid token returns true")]
        public async Task ValidateTokenGivenValidTokenReturnsTrue()
        {
            // Arrange
            var user = testuser;

            var claim = new Claim(ClaimTypes.Email, user.Email);
            var claims = new List<Claim> { claim };

            bool result;

            var context = GenerateCoffeeCardContext(nameof(ValidateTokenGivenValidTokenReturnsTrue));
            await using (context)
            {
                var claimsUtility = new ClaimsUtilities(context);
                var tokenService = new TokenService(_identity, claimsUtility);

                var token = tokenService.GenerateToken(claims);
                var userTokens = new List<Token> { new Token(token) };
                user.Tokens = userTokens;
                await context.AddAsync(user);
                await context.SaveChangesAsync();

                // Act
                result = await tokenService.ValidateTokenIsUnusedAsync(token);
            }

            // Assert
            Assert.True(result);
        }

        [Fact(DisplayName = "ValidateToken given invalid signed token returns false")]
        public async Task ValidateTokenGivenInvalidSignedTokenReturnsFalse()
        {
            // Arrange
            var claim = new Claim(ClaimTypes.Email, "test@email.dk");
            var claims = new List<Claim> { claim };

            var context = GenerateCoffeeCardContext(nameof(ValidateTokenGivenInvalidSignedTokenReturnsFalse));
            await using (context)
            {
                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("Invalid signing key"));

                var jwt = new JwtSecurityToken("AnalogIO",
                    "Everyone",
                    claims,
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddHours(24),
                    new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

                var token = new JwtSecurityTokenHandler().WriteToken(jwt);
                var claimsUtility = new ClaimsUtilities(context);
                var tokenService = new TokenService(_identity, claimsUtility);

                // Act
                var result = tokenService.ValidateToken(token);

                // Assert
                Assert.False(result);
            }
        }

        [Fact(DisplayName = "ValidateToken given welformed expired token returns false")]
        public async Task ValidateTokenGivenWelformedExpiredTokenReturnsFalse()
        {
            // Arrange
            var user = testuser;
            var claim = new Claim(ClaimTypes.Email, user.Email);
            var claims = new List<Claim> { claim };

            bool result;

            var context = GenerateCoffeeCardContext(nameof(ValidateTokenGivenWelformedExpiredTokenReturnsFalse));
            {
                await using (context)
                {
                    var claimsUtility = new ClaimsUtilities(context);
                    var tokenService = new TokenService(_identity, claimsUtility);

                    var key = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_identity.TokenKey));

                    //Creates the expired token
                    var jwt = new JwtSecurityToken("AnalogIO",
                        "Everyone",
                        claims,
                        DateTime.UtcNow.Subtract(new TimeSpan(1200)),
                        DateTime.UtcNow.Subtract(new TimeSpan(600)),
                        new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                    );

                    var token = new JwtSecurityTokenHandler().WriteToken(jwt);

                    var userTokens = new List<Token> { new Token(token) };
                    user.Tokens = userTokens;
                    await context.AddAsync(user);
                    await context.SaveChangesAsync();

                    // Act
                    result = await tokenService.ValidateTokenIsUnusedAsync(token);
                }
            }

            // Assert
            Assert.False(result);
        }

        [Fact(DisplayName = "ValidateToken given welformed used token returns false")]
        public async Task ValidateTokenGivenWelformedUsedTokenReturnsFalse()
        {
            // Arrange
            var user = testuser;

            var claim = new Claim(ClaimTypes.Email, user.Email);
            var claims = new List<Claim> { claim };

            bool result;

            var context = GenerateCoffeeCardContext(nameof(ValidateTokenGivenWelformedUsedTokenReturnsFalse));
            await using (context)
            {
                var claimsUtility = new ClaimsUtilities(context);
                var tokenService = new TokenService(_identity, claimsUtility);

                var token = tokenService.GenerateToken(claims);

                // No tokens are added to the users list, therefore all tokens with a claim for this user will be assumed to be expired
                await context.AddAsync(user);
                await context.SaveChangesAsync();

                // Act
                result = await tokenService.ValidateTokenIsUnusedAsync(token);
            }

            // Assert
            Assert.False(result);
        }

        private CoffeeCardContext GenerateCoffeeCardContext(string uniqueString)
        {
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(uniqueString);

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
    }
}