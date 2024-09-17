using System;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2;

public class TokenService : ITokenService
{
    private readonly CoffeeCardContext _context;

    public TokenService(CoffeeCardContext context)
    {
        _context = context;
    }

    public string GenerateMagicLink(string email)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            throw new ApiException("No user found with the given email.");
        }
        
        var guid = Guid.NewGuid().ToString();
        var magicLinkToken = new Token(guid, TokenType.MagicLink, TokenType.MagicLink.getExpiresAt());
        
        user.Tokens.Add(magicLinkToken);
        _context.SaveChangesAsync();
        return magicLinkToken.TokenHash;
    }

    public async Task<string> GenerateRefreshTokenAsync(User user)
    {
        var refreshToken = new Guid().ToString();
        _context.Tokens.Add(new Token(refreshToken, TokenType.Refresh, TokenType.Refresh.getExpiresAt()));
        await _context.SaveChangesAsync();
        return refreshToken;
    }
}