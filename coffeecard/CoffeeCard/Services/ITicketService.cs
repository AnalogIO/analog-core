using System.Collections.Generic;
using System.Security.Claims;
using CoffeeCard.WebApi.Models;
using CoffeeCard.WebApi.Models.DataTransferObjects.Ticket;

namespace CoffeeCard.WebApi.Services
{
    public interface ITicketService
    {
        IEnumerable<Ticket> getTickets(IEnumerable<Claim> claims, bool used);
        Ticket UseTicket(IEnumerable<Claim> claims, int productId);
        IEnumerable<Ticket> UseMultipleTickets(IEnumerable<Claim> claims, UseMultipleTicketDTO dto);
        IEnumerable<CoffeCard> GetCoffeCards(IEnumerable<Claim> claims);
    }
}