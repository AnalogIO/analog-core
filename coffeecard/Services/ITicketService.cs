using Coffeecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace coffeecard.Services
{
    public interface ITicketService
    {
        IEnumerable<Ticket> getTickets(IEnumerable<Claim> claims, bool used);
    }
}
