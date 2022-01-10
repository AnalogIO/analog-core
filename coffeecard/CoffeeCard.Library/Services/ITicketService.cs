using System.Collections.Generic;
using System.Security.Claims;
using CoffeeCard.Models.DataTransferObjects.CoffeeCard;
using CoffeeCard.Models.DataTransferObjects.Ticket;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services
{
    public interface ITicketService
    {
        IEnumerable<Ticket> GetTickets(IEnumerable<Claim> claims, bool used);
        Ticket UseTicket(IEnumerable<Claim> claims, int productId);
        IEnumerable<Ticket> UseMultipleTickets(IEnumerable<Claim> claims, UseMultipleTicketDto dto);
        IEnumerable<CoffeeCardDto> GetCoffeeCards(IEnumerable<Claim> claims);
    }
}