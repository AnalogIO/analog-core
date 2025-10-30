using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoffeeCard.Library.Services
{
    public interface ITokenService
    {
        string GenerateToken(IEnumerable<Claim> claims);
        JwtSecurityToken ReadToken(string token);
        bool ValidateToken(string tokenString);
        Task<bool> ValidateTokenIsUnusedAsync(string tokenString);
        string ValidateVerificationTokenAndGetEmail(string token);
    }
}
