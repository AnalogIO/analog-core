using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.Common;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Library.Utils
{
    public class ClaimsUtilities
    {
        private readonly CoffeeCardContext _context;

        public ClaimsUtilities(CoffeeCardContext context)
        {
            _context = context;
        }

        public async Task<User> ValidateAndReturnUserFromClaimAsync(IEnumerable<Claim> claims)
        {
            var userId = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userId == null)
                throw new ApiException("The token is invalid!", 401);

            var user = await _context
                .Users.Where(u => u.Id == int.Parse(userId.Value))
                .Include(x => x.Purchases)
                .FirstOrDefaultAsync();
            if (user == null)
                throw new ApiException(
                    "The user could not be found",
                    StatusCodes.Status404NotFound
                );

            return user;
        }

        public async Task<User> ValidateAndReturnUserFromEmailClaimAsync(IEnumerable<Claim> claims)
        {
            var emailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            if (emailClaim == null)
                throw new ApiException("The token is invalid!", 401);
            var email = emailClaim.Value;

            var user = await _context
                .Users.Where(u => u.Email == email)
                .Include(u => u.Tokens)
                .Include(u => u.Programme)
                .FirstOrDefaultAsync();
            if (user == null)
                throw new ApiException(
                    "The user could not be found",
                    StatusCodes.Status404NotFound
                );

            return user;
        }
    }
}
