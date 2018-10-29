using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using coffeecard.Helpers;
using Coffeecard.Models;

namespace coffeecard.Services
{
    public class TicketService : ITicketService
    {

        private readonly CoffeecardContext _context;

        public TicketService(CoffeecardContext context)
        {
            _context = context;
        }

        public IEnumerable<Ticket> getTickets(IEnumerable<Claim> claims, bool used)
        {
            var userId = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userId == null) throw new ApiException($"The token is invalid!", 401);
            var id = int.Parse(userId.Value);
            return _context.Tickets.Where(x => x.Owner.Id == id && x.IsUsed == used);
        }
    }
}
