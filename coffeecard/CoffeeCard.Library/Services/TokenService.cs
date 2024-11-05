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
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.CoffeeCard;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace CoffeeCard.Library.Services
{
    public class TokenService : ITokenService
    {
        private readonly ClaimsUtilities _claimsUtilities;
        private readonly IdentitySettings _identitySettings;

        public TokenService(IdentitySettings identitySettings, ClaimsUtilities claimsUtilities)
        {
            _identitySettings = identitySettings;
            _claimsUtilities = claimsUtilities;
        }

        public string GenerateToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_identitySettings.TokenKey)); // get token from appsettings.json

            var jwt = new JwtSecurityToken("AnalogIO",
                "Everyone",
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(1),
                new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(jwt); //the method is called WriteToken but returns a string
        }

        public JwtSecurityToken ReadToken(string token)
        {
            return new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
        }


        /// <summary>
        /// Receives the serialized version of a JWT token as a string.
        /// Checks if the JWT token is valid (Based on its lifetime)
        /// </summary>
        /// <param name="tokenString"></param>
        /// <returns>bool</returns>
        public bool ValidateToken(string tokenString)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_identitySettings.TokenKey));

            try
            {
                var securityTokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero //the default for this setting is 5 minutes
                };

                securityTokenHandler.ValidateToken(tokenString, validationParameters, out _); // Throws exception if token is invalid
            }
            catch (Exception e) when (e is ArgumentException ||
                                      e is SecurityTokenException)
            {
                Log.Information("Received invalid token");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Receives the serialized version of a JWT token as a string.
        /// Checks if the JWT token is valid (Based on its lifetime and if it has been used before)
        /// </summary>
        /// <param name="tokenString"></param>
        /// <returns>bool</returns>
        public async Task<bool> ValidateTokenIsUnusedAsync(string tokenString)
        {
            try
            {
                var tokenIsUnused = false;
                var token = ReadToken(tokenString);

                var user = await _claimsUtilities.ValidateAndReturnUserFromEmailClaimAsync(token.Claims);

                if (user.Tokens.Any((e) => e.TokenHash == tokenString)) tokenIsUnused = true; // Tokens are removed from the user on account recovery

                return ValidateToken(tokenString) && tokenIsUnused;
            }
            catch (ArgumentException e)
            {
                Log.Error(e, "Unable to read token. Exception thrown");
                return false;
            }
        }

        public string ValidateVerificationTokenAndGetEmail(string token)
        {
            if (!ValidateToken(token))
            {
                Log.Information("Token validation failed. DId not pass validation parameters");
                throw new ApiException("The token is invalid!", StatusCodes.Status401Unauthorized);
            }

            var jwtToken = ReadToken(token);
            if (jwtToken.Claims.Any(x => x.Type == ClaimTypes.Role && x.Value != "verification_token"))
            {
                Log.Information("Token validation failed. Not a verification token");
                throw new ApiException("The token is invalid!", StatusCodes.Status401Unauthorized);
            }

            var emailClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            if (emailClaim == null)
            {
                Log.Information("Token validation failed. No email found in token");
                throw new ApiException("The token is invalid!", StatusCodes.Status401Unauthorized);
            }

            return emailClaim.Value;
        }
    }
}