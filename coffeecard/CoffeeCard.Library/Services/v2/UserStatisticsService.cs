using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.DataTransferObjects.v2.UserStatistics;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Library.Services.v2
{
    public class UserStatisticsService : IUserStatisticsService
    {
        private readonly CoffeeCardContext _context;

        public UserStatisticsService(CoffeeCardContext context)
        {
            _context = context;
        }

        public async Task<List<UnusedClipsResponse>> GetUnusedClips(UnusedClipsRequest unusedClipsRequest)
        {
            var startDate = unusedClipsRequest.StartDate;
            var endDate = unusedClipsRequest.EndDate;

            var tickets = await _context.Tickets
                .Where(t => t.DateCreated >= startDate && t.DateCreated <= endDate && t.IsUsed == false)
                .Join(_context.Purchases,
                    ticket => ticket.PurchaseId,
                    purchase => purchase.Id,
                    (ticket, purchase) => new { ticket, purchase })
                .GroupBy(combined => combined.purchase.ProductId)
                .Select(group => new UnusedClipsResponse
                {
                    ProductId = group.Key,
                    ProductName = group.First().purchase.ProductName,
                    TicketsLeft = group.Count(),
                    ToRefund = group.Sum(item => (1 / (decimal)item.purchase.NumberOfTickets) * item.purchase.Price),
                })
                .ToListAsync();


            return tickets;
        }
    }
}