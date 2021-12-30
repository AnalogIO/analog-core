using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.Common;
using CoffeeCard.Common.Errors;
using CoffeeCard.Common.Models;
using CoffeeCard.Library.Persistence;
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
            if (userId == null) throw new ApiException("The token is invalid!", 401);

            var user = await _context.Users.Include(x => x.Purchases)
                .FirstOrDefaultAsync(x => x.Id == int.Parse(userId.Value));
            if (user == null) throw new ApiException("The user could not be found");

            return user;
        }

        public async Task<User> ValidateAndReturnUserFromEmailClaimAsync(IEnumerable<Claim> claims)
        {
            var emailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            if (emailClaim == null) throw new ApiException("The token is invalid!", 401);
            var email = emailClaim.Value;

            var user = await _context.Users.Include(x => x.Tokens).FirstOrDefaultAsync(x => x.Email == email);
            if (user == null) throw new ApiException("The user could not be found");

            return user;
        }
    }
}