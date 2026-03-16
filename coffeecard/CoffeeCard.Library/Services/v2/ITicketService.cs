using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.GroupedTicketsResponse;
using CoffeeCard.Models.DataTransferObjects.v2.Ticket;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2
{
    public interface ITicketService : IDisposable
    {
        Task IssueTickets(Purchase purchase);

        Task<IEnumerable<TicketResponse>> GetTicketsAsync(User user, bool includeUsed);

        Task<IEnumerable<GroupedTicketsResponse>> GetGroupedTicketsAsync(User user);

        public Task<UsedTicketResponse> UseTicketAsync(User user, int productId);

        public Task<UsedTicketResponse> UseTicketAsync(User user, int productId, int menuItemId);
    }
}
