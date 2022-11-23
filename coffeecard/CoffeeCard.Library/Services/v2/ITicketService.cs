using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.Ticket;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2
{
    public interface ITicketService
    {
        Task IssueTickets(Purchase purchase);

        Task<List<TicketResponse>> GetTickets(User user, bool includeUsed);

        public Task<UsedTicketResponse> UseTicketAsync(User user, int productId);
    }
}