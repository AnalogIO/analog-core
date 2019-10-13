using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["TokenKey"])); // get token from appsettings.json

        var jwt = new JwtSecurityToken(issuer: "AnalogIO",
            audience: "Everyone",
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt); //the method is called WriteToken but returns a string
    }

    public JwtSecurityToken ReadToken(string token)
    {
        return new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
    }

    /// <summary>
    /// Checks if the JWT token is valid (Based on its lifetime)
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public bool ValidateToken(JwtSecurityToken token)
    {
        try
        {
            if (token.ValidTo > DateTime.UtcNow) return true;
            else return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
}