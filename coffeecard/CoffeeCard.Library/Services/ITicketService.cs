using System.Collections.Generic;
using System.Security.Claims;
using CoffeeCard.Common.Models;
using CoffeeCard.Common.Models.DataTransferObjects.Ticket;

namespace CoffeeCard.Library.Services
{
    public interface ITicketService
    {
        IEnumerable<Ticket> getTickets(IEnumerable<Claim> claims, bool used);
        Ticket UseTicket(IEnumerable<Claim> claims, int productId);
        IEnumerable<Ticket> UseMultipleTickets(IEnumerable<Claim> claims, UseMultipleTicketDto dto);
        IEnumerable<Common.Models.CoffeeCard> GetCoffeeCards(IEnumerable<Claim> claims);
    }
}