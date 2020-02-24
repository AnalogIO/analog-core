using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Helpers
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

            var user = await _context.Users.Include(x => x.Purchases).FirstOrDefaultAsync(x => x.Id == int.Parse(userId.Value));
            if (user == null) throw new ApiException("The user could not be found");

            return user;
        }
    }
}
