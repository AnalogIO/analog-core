using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.CoffeeCard;
using CoffeeCard.Models.DataTransferObjects.Ticket;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services
{
    public interface ITicketService
    {
        IEnumerable<Ticket> GetTickets(IEnumerable<Claim> claims, bool used);
        Task<Ticket> UseTicket(IEnumerable<Claim> claims, int productId);
        Task<IEnumerable<Ticket>> UseMultipleTickets(
            IEnumerable<Claim> claims,
            UseMultipleTicketDto dto
        );
        IEnumerable<CoffeeCardDto> GetCoffeeCards(IEnumerable<Claim> claims);
    }
}
