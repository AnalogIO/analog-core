using System;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;

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
            throw new ApiException("No user found with the given email."); // This check is already done in AccountService
        }

        var guid = Guid.NewGuid().ToString();
        var magicLinkToken = new Token(guid, TokenType.MagicLink, TokenType.MagicLink.getExpiresAt());

        user.Tokens.Add(magicLinkToken);
        _context.SaveChangesAsync();
        return magicLinkToken.TokenHash;
    }

    public async Task<string> GenerateRefreshTokenAsync(User user)
    {
        var refreshToken = Guid.NewGuid().ToString();
        _context.Tokens.Add(new Token(refreshToken, TokenType.Refresh, TokenType.Refresh.getExpiresAt()));
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<string> ValidateTokenAsync(string refreshToken)
    {
        var token = await _context.Tokens.FirstOrDefaultAsync(t => t.TokenHash == refreshToken);
        if (token.Revoked)
        {
            // TODO: Invalidate chain of tokens
            throw new ApiException("Refresh token is already used", 401);
        }
        throw new NotImplementedException();
    }
}