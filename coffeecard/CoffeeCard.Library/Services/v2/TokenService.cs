using System;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Library.Services.v2;

public class TokenService : ITokenService
{
    private readonly CoffeeCardContext _context;
    private readonly IHashService _hashService;

    public TokenService(CoffeeCardContext context, IHashService hashService)
    {
        _context = context;
        _hashService = hashService;
    }

    public string GenerateMagicLink(User user)
    {
        var guid = Guid.NewGuid().ToString();
        var magicLinkToken = new Token(guid, TokenType.MagicLink);

        user.Tokens.Add(magicLinkToken);
        _context.SaveChangesAsync();
        return magicLinkToken.TokenHash;
    }

    public async Task<string> GenerateRefreshTokenAsync(User user)
    {
        var refreshToken = Guid.NewGuid().ToString();
        var hashedToken = _hashService.Hash(refreshToken);
        _context.Tokens.Add(new Token(hashedToken, TokenType.Refresh) { User = user });
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<Token> GetValidTokenByHashAsync(string tokenString)
    {
        var tokenHash = _hashService.Hash(tokenString);
        var foundToken = await _context.Tokens.Include(t => t.User).FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
        if (foundToken == null || foundToken.Revoked || foundToken.Expired())
        {
            await InvalidateRefreshTokensForUser(foundToken?.User);
            throw new ApiException("Invalid token", 401);
        }
        return foundToken;
    }

    private async Task InvalidateRefreshTokensForUser(User user)
    {
        if (user is null) return;

        var tokens = _context.Tokens.Where(t => t.UserId == user.Id && t.Type == TokenType.Refresh);

        _context.Tokens.UpdateRange(tokens);
        foreach (var token in tokens)
        {
            token.Revoked = true;
        }
        await _context.SaveChangesAsync();
    }
}