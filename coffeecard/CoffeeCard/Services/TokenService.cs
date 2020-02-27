using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoffeeCard.Common.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CoffeeCard.WebApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly IdentitySettings _identitySettings;

        public TokenService(IdentitySettings identitySettings)
        {
            _identitySettings = identitySettings;
        }

        public string GenerateToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_identitySettings.TokenKey)); // get token from appsettings.json

            var jwt = new JwtSecurityToken("AnalogIO",
                "Everyone",
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(24),
                new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt); //the method is called WriteToken but returns a string
        }

        public JwtSecurityToken ReadToken(string token)
        {
            return new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
        }

        /// <summary>
        ///     Checks if the JWT token is valid (Based on its lifetime)
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool ValidateToken(JwtSecurityToken token)
        {
            try
            {
                if (token.ValidTo > DateTime.UtcNow) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}