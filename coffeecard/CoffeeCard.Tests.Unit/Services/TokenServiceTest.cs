using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.WebApi.Helpers;
using CoffeeCard.WebApi.Models;
using CoffeeCard.WebApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services
{
    public class TokenServiceTest
    {
        private readonly CoffeeCardContext _context;
        private readonly IdentitySettings _identity;

        public TokenServiceTest()
        {
            _identity = new IdentitySettings();
            
            //creates the key for signing the token
            byte[] keyForHmacSha256 = new byte[64];
            var randomGen = RandomNumberGenerator.Create();
            randomGen.GetBytes(keyForHmacSha256);
            _identity.TokenKey = new SymmetricSecurityKey(keyForHmacSha256).ToString();
            
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(TokenServiceTest));

            var databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };
            
            _context = new CoffeeCardContext(builder.Options, databaseSettings);
        }
        
        [Fact(DisplayName = "ValidateToken given valid token returns true")]
        public async Task ValidateTokenGivenValidTokenReturnsTrue()
        {
            // Arrange
            var claim = new Claim(ClaimTypes.Email,"test@email.dk");
            var claims = new List<Claim>() {claim};

            bool result;

            await using (_context)
            {
                var claimsUtility = new ClaimsUtilities(_context);
                var tokenService = new TokenService(_identity, claimsUtility);

                var token = tokenService.GenerateToken(claims);
                var userTokens = new List<Token>() { new Token(token)};
                var user = new User {Tokens = userTokens, Email = "test@email.dk"};
                _context.Add(user);
                _context.SaveChanges();
                
                // Act
                result = await tokenService.ValidateToken(token);
            }
            
            // Assert
            Assert.True(result);
        }
        
        [Fact(DisplayName = "ValidateToken given invalid token returns false")]
        public async Task ValidateTokenGivenInvalidTokenReturnsFalse()
        {
            // Arrange
            var claim = new Claim(ClaimTypes.Email,"test@email.dk");
            var claims = new List<Claim>() {claim};
            
            bool result;

            await using (_context)
            {
                var claimsUtility = new ClaimsUtilities(_context);
                var tokenService = new TokenService(_identity, claimsUtility);

                //creates the token
                var token = tokenService.GenerateToken(claims);
                
                var userTokens = new List<Token>() { new Token(token)};
                var user = new User {Tokens = userTokens, Email = "test@email.dk"};
                _context.Add(user);
                _context.SaveChanges();
                
                // Act
                result = await tokenService.ValidateToken("Bogus token");
            }
            
            // Assert
            Assert.False(result);
        }
        
        [Fact(DisplayName = "ValidateToken given welformed expired token returns false")]
        public async Task ValidateTokenGivenWelformedExpiredTokenReturnsFalse()
        {
            // Arrange
            var claim = new Claim(ClaimTypes.Email,"test@email.dk");
            var claims = new List<Claim>() {claim};

            bool result;

            await using (_context)
            {
                var claimsUtility = new ClaimsUtilities(_context);
                var tokenService = new TokenService(_identity, claimsUtility);
                
                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_identity.TokenKey));
                
                //Creates the expired token
                var jwt = new JwtSecurityToken("AnalogIO",
                    "Everyone",
                    claims,
                    DateTime.UtcNow.Subtract(new TimeSpan(120)),
                    DateTime.UtcNow.Subtract(new TimeSpan(60)),
                    new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

                var token = new JwtSecurityTokenHandler().WriteToken(jwt);
                
                var userTokens = new List<Token>() { new Token(token)};
                var user = new User {Tokens = userTokens, Email = "test@email.dk"};
                _context.Add(user);
                _context.SaveChanges();
                
                // Act
                result = await tokenService.ValidateToken(token);
            }
            
            // Assert
            Assert.False(result);
        }
        
        [Fact(DisplayName = "ValidateToken given welformed used token returns false")]
        public async Task ValidateTokenGivenWelformedUsedTokenReturnsFalse()
        {
            // Arrange
            var claim = new Claim(ClaimTypes.Email,"test@email.dk");
            var claims = new List<Claim>() {claim};

            bool result;

            await using (_context)
            {
                var claimsUtility = new ClaimsUtilities(_context);
                var tokenService = new TokenService(_identity, claimsUtility);
                
                var token = tokenService.GenerateToken(claims);

                var userTokens = new List<Token>(); //No tokens are added to the users list, therefore all tokens with a claim for this user will be assumed to be expired
                var user = new User {Tokens = userTokens, Email = "test@email.dk"};
                _context.Add(user);
                _context.SaveChanges();
                
                // Act
                result = await tokenService.ValidateToken(token);
            }
            
            // Assert
            Assert.False(result);
        }
    }
}