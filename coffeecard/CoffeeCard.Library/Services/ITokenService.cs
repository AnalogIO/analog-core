using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoffeeCard.Library.Services
{
    public interface ITokenService
    {
        string GenerateToken(IEnumerable<Claim> claims);
        string GenerateRegistrationToken(string email);
        string ValidateRegistrationTokenAndGetEmail(string token);
        JwtSecurityToken ReadToken(string token);
        bool ValidateToken(string tokenString);
        Task<bool> ValidateTokenIsUnusedAsync(string tokenString);
        string ValidateVerificationTokenAndGetEmail(string token);
    }
}
