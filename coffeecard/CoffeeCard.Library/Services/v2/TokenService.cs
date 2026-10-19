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

    public async Task<string> GenerateMagicLinkToken(User user)
    {
        var guid = Guid.NewGuid().ToString();
        var magicLinkToken = new Token(_hashService.Hash(guid), TokenType.MagicLink)
        {
            User = user,
        };
        _context.Tokens.Add(magicLinkToken);

        await _context.SaveChangesAsync();
        return guid;
    }

    public async Task<string> GenerateRefreshTokenAsync(User user)
    {
        var refreshToken = Guid.NewGuid().ToString();
        var hashedToken = _hashService.Hash(refreshToken);
        _context.Tokens.Add(new Token(hashedToken, TokenType.Refresh) { UserId = user.Id });
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<string> GenerateRegistrationTokenAsync(string email)
    {
        // Only the newest link should work. This also prevents an old link from
        // being usable after the user requests a new one.
        var previousTokens = await _context
            .Tokens.Where(t => t.Type == TokenType.Registration && t.Email == email && !t.Revoked)
            .ToListAsync();

        foreach (var previousToken in previousTokens)
        {
            previousToken.Revoked = true;
        }

        var registrationToken = Guid.NewGuid().ToString();
        _context.Tokens.Add(
            new Token(_hashService.Hash(registrationToken), TokenType.Registration)
            {
                Email = email,
            }
        );

        await _context.SaveChangesAsync();
        return registrationToken;
    }

    public async Task<Token> GetValidTokenByHashAsync(string tokenString)
    {
        var tokenHash = _hashService.Hash(tokenString);
        var foundToken = await _context
            .Tokens.Include(t => t.User)
            .FirstOrDefaultAsync(t =>
                t.TokenHash == tokenHash && !t.Revoked && !Token.Expired(t.Expires)
            );

        if (foundToken == null)
        {
            throw new UnauthorizedException("Invalid token");
        }
        return foundToken;
    }

    public async Task<Token> GetValidRegistrationTokenAsync(string tokenString)
    {
        var tokenHash = _hashService.Hash(tokenString);
        var foundToken = await _context.Tokens.FirstOrDefaultAsync(t =>
            t.TokenHash == tokenHash
            && !t.Revoked
            && !Token.Expired(t.Expires)
            && t.Type == TokenType.Registration
            && t.Email != null
        );

        if (foundToken == null)
        {
            throw new UnauthorizedException("Invalid token");
        }

        return foundToken;
    }
}
