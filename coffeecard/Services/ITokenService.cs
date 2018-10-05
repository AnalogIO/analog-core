using System.Collections.Generic;
using System.Security.Claims;

public interface ITokenService
{
    string GenerateToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}