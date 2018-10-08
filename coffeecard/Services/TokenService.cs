using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

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
            claims: claims, //the user's claims, for example new Claim[] { new Claim(ClaimTypes.Name, "The username"), //... 
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );    

        return new JwtSecurityTokenHandler().WriteToken(jwt); //the method is called WriteToken but returns a string
    }
}