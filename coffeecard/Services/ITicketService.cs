using coffeecard.Models.DataTransferObjects.Ticket;
using Coffeecard.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace coffeecard.Services
{
    public interface ITicketService
    {
        IEnumerable<Ticket> getTickets(IEnumerable<Claim> claims, bool used);
        Ticket UseTicket(IEnumerable<Claim> claims, int ticketId);
        IEnumerable<Ticket> UseMultipleTickets(IEnumerable<Claim> claims, int[] ticketIds);
    }
}
