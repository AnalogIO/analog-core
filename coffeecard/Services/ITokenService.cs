using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public interface ITokenService
{
    string GenerateToken(IEnumerable<Claim> claims);
    JwtSecurityToken ReadToken(string token);
}