using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.DataTransferObjects.v2.AdminStatistics;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Library.Services.v2
{
    public class AdminStatisticsService : IAdminStatisticsService
    {
        private readonly CoffeeCardContext _context;

        public AdminStatisticsService(CoffeeCardContext context)
        {
            _context = context;
        }

        public async Task<List<UnusedClipsResponse>> GetUsableClips(
            UnusedClipsRequest unusedClipsRequest
        )
        {
            var startDate = unusedClipsRequest.StartDate;
            var endDate = unusedClipsRequest.EndDate;

            var tickets = await _context
                .Tickets.Where(t =>
                    t.DateCreated >= startDate
                    && t.DateCreated <= endDate
                    && t.Status == TicketStatus.Unused
                )
                .GroupBy(ticket => ticket.ProductId)
                .Select(group => new UnusedClipsResponse
                {
                    ProductId = group.Key,
                    ProductName = group.First().Purchase.ProductName,
                    TicketsLeft = group.Count(),
                    UnusedPurchasesValue = group.Sum(item =>
                        (1 / (decimal)item.Purchase.NumberOfTickets) * item.Purchase.Price
                    ),
                })
                .ToListAsync();

            return tickets;
        }
    }
}
