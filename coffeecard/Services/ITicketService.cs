using System.Collections.Generic;
using System.Security.Claims;
using CoffeeCard.Models;
using CoffeeCard.Models.DataTransferObjects.Ticket;

namespace CoffeeCard.Services
{
    public interface ITicketService
    {
        IEnumerable<Ticket> getTickets(IEnumerable<Claim> claims, bool used);
        Ticket UseTicket(IEnumerable<Claim> claims, int productId);
        IEnumerable<Ticket> UseMultipleTickets(IEnumerable<Claim> claims, UseMultipleTicketDTO dto);
        IEnumerable<CoffeCard> GetCoffeCards(IEnumerable<Claim> claims);
    }
}
