using System;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.Entities;
using Microsoft.Data.SqlClient;
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
        const int maxAttempts = 3;

        for (var attempt = 1; ; attempt++)
        {
            var registrationToken = Guid.NewGuid().ToString();
            var token = new Token(_hashService.Hash(registrationToken), TokenType.Registration)
            {
                Email = email,
            };

            try
            {
                await GenerateRegistrationTokenOnceAsync(token, email);
                return registrationToken;
            }
            catch (DbUpdateException exception)
                when (attempt < maxAttempts && IsUniqueConstraintViolation(exception))
            {
                // A concurrent request inserted a newer link first. Drop the
                // failed insert and try again; the next attempt revokes the
                // winner so only the newest link remains usable.
                _context.Entry(token).State = EntityState.Detached;
            }
        }
    }

    private async Task GenerateRegistrationTokenOnceAsync(Token token, string email)
    {
        // Only the newest link should work. This also prevents an old link from
        // being usable after the user requests a new one.
        var previousTokensQuery = _context.Tokens.Where(t =>
            t.Type == TokenType.Registration && t.Email == email && !t.Revoked
        );

        if (_context.Database.IsRelational())
        {
            // Revocation and insertion must commit together. The filtered unique
            // index on active registration tokens prevents a concurrent request
            // from leaving two usable links.
            await using var transaction = await _context.Database.BeginTransactionAsync();

            await previousTokensQuery.ExecuteUpdateAsync(t =>
                t.SetProperty(previous => previous.Revoked, true)
            );

            _context.Tokens.Add(token);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return;
        }

        // The in-memory provider supports neither transactions nor ExecuteUpdate.
        foreach (var previousToken in await previousTokensQuery.ToListAsync())
        {
            previousToken.Revoked = true;
        }

        _context.Tokens.Add(token);
        await _context.SaveChangesAsync();
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        // SQL Server reports duplicate keys for unique indexes as 2601/2627.
        return exception.InnerException is SqlException { Number: 2601 or 2627 };
    }

    public Task<Token> GetValidTokenByHashAsync(string tokenString)
    {
        // /account/auth accepts both magic-link and refresh tokens.
        // Registration tokens have no user yet and must stay out of this lookup.
        return GetValidTokenAsync(
            tokenString,
            [TokenType.MagicLink, TokenType.Refresh],
            includeUser: true
        );
    }

    public async Task<Token> GetValidRegistrationTokenAsync(string tokenString)
    {
        var foundToken = await GetValidTokenAsync(
            tokenString,
            [TokenType.Registration],
            includeUser: false
        );

        if (foundToken.Email is null)
        {
            throw new UnauthorizedException("Invalid token");
        }

        return foundToken;
    }

    private async Task<Token> GetValidTokenAsync(
        string tokenString,
        TokenType[] types,
        bool includeUser
    )
    {
        var tokenHash = _hashService.Hash(tokenString);
        var query = _context.Tokens.Where(t =>
            t.TokenHash == tokenHash
            && !t.Revoked
            && !Token.Expired(t.Expires)
            && types.Contains(t.Type)
        );

        if (includeUser)
        {
            query = query.Include(t => t.User);
        }

        var foundToken = await query.FirstOrDefaultAsync();

        if (foundToken == null)
        {
            throw new UnauthorizedException("Invalid token");
        }

        return foundToken;
    }
}
