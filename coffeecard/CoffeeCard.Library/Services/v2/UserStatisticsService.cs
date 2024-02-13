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
                .GroupBy(t => t.PurchaseId)
                .Select(g => new
                {
                    PurchaseId = g.Key,
                    Count = g.Count()
                })
                .Join(_context.Purchases,
                    ticketGroup => ticketGroup.PurchaseId,
                    purchase => purchase.Id,
                    (ticketGroup, purchase) => new UnusedClipsResponse
                    {
                        PurchaseId = ticketGroup.PurchaseId,
                        TicketsLeft = ticketGroup.Count,
                        ToRefund = (ticketGroup.Count / (float)purchase.NumberOfTickets) * purchase.Price,

                    })
                .ToListAsync();


            return tickets;
        }
    }
}