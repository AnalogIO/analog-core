using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services
{
    public class TokenServiceTest
    {
        public TokenServiceTest()
        {
            _identity = new IdentitySettings();

            //creates the key for signing the token
            const string keyForHmacSha256 = "SuperLongSigningKey";
            _identity.TokenKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(keyForHmacSha256)).ToString();
        }

        private readonly IdentitySettings _identity;

        private static User GenerateTestUser(
            ICollection<Token> tokens,
            string email = "test@email.dk",
            string name = "test",
            string password = "1234",
            string salt = "salted"
        )
        {
            return new User { Tokens = tokens, Email = email, Name = name, Password = password, Salt = salt, Programme = new Programme { ShortName = "test", FullName = "tester", SortPriority = 1 } };
        }

        [Fact(DisplayName = "ValidateToken given invalid token returns false")]
        public async Task ValidateTokenGivenInvalidTokenReturnsFalse()
        {
            // Arrange
            bool result;

            CoffeeCardContext context = GenerateCoffeeCardContext(nameof(ValidateTokenGivenInvalidTokenReturnsFalse));
            await using (context)
            {
                ClaimsUtilities claimsUtility = new ClaimsUtilities(context);
                TokenService tokenService = new TokenService(_identity, claimsUtility);

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
            Claim claim = new Claim(ClaimTypes.Email, "test@email.dk");
            List<Claim> claims = [claim];

            bool result;

            CoffeeCardContext context = GenerateCoffeeCardContext(nameof(ValidateTokenGivenValidTokenReturnsTrue));
            await using (context)
            {
                ClaimsUtilities claimsUtility = new ClaimsUtilities(context);
                TokenService tokenService = new TokenService(_identity, claimsUtility);

                string token = tokenService.GenerateToken(claims);
                List<Token> userTokens = [new Token(token)];
                User user = GenerateTestUser(tokens: userTokens);
                _ = await context.AddAsync(user);
                _ = await context.SaveChangesAsync();

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
            Claim claim = new Claim(ClaimTypes.Email, "test@email.dk");
            List<Claim> claims = [claim];

            CoffeeCardContext context = GenerateCoffeeCardContext(nameof(ValidateTokenGivenInvalidSignedTokenReturnsFalse));
            await using (context)
            {
                SymmetricSecurityKey key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("Super long invalid signing key, longer than 256bytes"));

                JwtSecurityToken jwt = new JwtSecurityToken("AnalogIO",
                    "Everyone",
                    claims,
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddHours(24),
                    new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

                string token = new JwtSecurityTokenHandler().WriteToken(jwt);
                ClaimsUtilities claimsUtility = new ClaimsUtilities(context);
                TokenService tokenService = new TokenService(_identity, claimsUtility);

                // Act
                bool result = tokenService.ValidateToken(token);

                // Assert
                Assert.False(result);
            }
        }

        [Fact(DisplayName = "ValidateToken given welformed expired token returns false")]
        public async Task ValidateTokenGivenWelformedExpiredTokenReturnsFalse()
        {
            // Arrange
            Claim claim = new Claim(ClaimTypes.Email, "test@email.dk");
            List<Claim> claims = [claim];

            bool result;

            CoffeeCardContext context = GenerateCoffeeCardContext(nameof(ValidateTokenGivenWelformedExpiredTokenReturnsFalse));
            {
                await using (context)
                {
                    ClaimsUtilities claimsUtility = new ClaimsUtilities(context);
                    TokenService tokenService = new TokenService(_identity, claimsUtility);

                    SymmetricSecurityKey key = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_identity.TokenKey));

                    //Creates the expired token
                    JwtSecurityToken jwt = new JwtSecurityToken("AnalogIO",
                        "Everyone",
                        claims,
                        DateTime.UtcNow.Subtract(new TimeSpan(1200)),
                        DateTime.UtcNow.Subtract(new TimeSpan(600)),
                        new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                    );

                    string token = new JwtSecurityTokenHandler().WriteToken(jwt);

                    List<Token> userTokens = [new Token(token)];
                    User user = GenerateTestUser(tokens: userTokens);
                    _ = await context.AddAsync(user);
                    _ = await context.SaveChangesAsync();

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
            Claim claim = new Claim(ClaimTypes.Email, "test@email.dk");
            List<Claim> claims = [claim];

            bool result;

            CoffeeCardContext context = GenerateCoffeeCardContext(nameof(ValidateTokenGivenWelformedUsedTokenReturnsFalse));
            await using (context)
            {
                ClaimsUtilities claimsUtility = new ClaimsUtilities(context);
                TokenService tokenService = new TokenService(_identity, claimsUtility);

                string token = tokenService.GenerateToken(claims);

                List<Token> userTokens =
                    []; //No tokens are added to the users list, therefore all tokens with a claim for this user will be assumed to be expired
                User user = GenerateTestUser(tokens: userTokens);
                _ = await context.AddAsync(user);
                _ = await context.SaveChangesAsync();

                // Act
                result = await tokenService.ValidateTokenIsUnusedAsync(token);
            }

            // Assert
            Assert.False(result);
        }

        private CoffeeCardContext GenerateCoffeeCardContext(string uniqueString)
        {
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(uniqueString);

            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            return new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
        }
    }
}
