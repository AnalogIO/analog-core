using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.v2.Statistics;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Library.Services.v2
{
    public class StatisticsService : IStatisticsService
    {
        private readonly CoffeeCardContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public StatisticsService(CoffeeCardContext context, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IEnumerable<QuickStatResponse>> GetQuickStatsAsync(User user)
        {
            var utcNow = _dateTimeProvider.UtcNow();
            var todayStart = utcNow.Date;
            var todayEnd = todayStart.AddDays(1);
            var weekStart = GetWeekStart(utcNow);
            var weekEnd = weekStart.AddDays(7);

            var totalDrinks = await _context.Tickets.CountAsync(ticket =>
                ticket.OwnerId == user.Id && ticket.Status == TicketStatus.Used
            );

            var drinksToday = await _context.Tickets.CountAsync(ticket =>
                ticket.Status == TicketStatus.Used
                && ticket.DateUsed >= todayStart
                && ticket.DateUsed < todayEnd
            );

            var favouriteDrink = await _context
                .Tickets.Where(ticket =>
                    ticket.OwnerId == user.Id
                    && ticket.Status == TicketStatus.Used
                    && ticket.UsedOnMenuItemId != null
                )
                .GroupBy(ticket => ticket.UsedOnMenuItem.Name)
                .Select(group => new { MenuItemName = group.Key, Count = group.Count() })
                .OrderByDescending(group => group.Count)
                .ThenBy(group => group.MenuItemName)
                .FirstOrDefaultAsync();

            var drinksThisWeek = await _context.Tickets.CountAsync(ticket =>
                ticket.OwnerId == user.Id
                && ticket.Status == TicketStatus.Used
                && ticket.DateUsed >= weekStart
                && ticket.DateUsed < weekEnd
            );

            return new List<QuickStatResponse>
            {
                new QuickStatResponse
                {
                    Key = "total-drinks-user",
                    Title = "Drinks consumed by you",
                    Value = totalDrinks,
                    SupportingText = null,
                },
                new QuickStatResponse
                {
                    Key = "global-drinks-today",
                    Title = "Drinks consumed by ITU today",
                    Value = drinksToday,
                    SupportingText = null,
                },
                new QuickStatResponse
                {
                    Key = "favourite-drink",
                    Title = "Your favourite drink",
                    Value = favouriteDrink?.Count ?? 0,
                    SupportingText = favouriteDrink?.MenuItemName ?? "No drinks yet",
                },
                new QuickStatResponse
                {
                    Key = "drinks-this-week-user",
                    Title = "Drinks this week",
                    Value = drinksThisWeek,
                    SupportingText = null,
                },
            };
        }

        private static DateTime GetWeekStart(DateTime utcNow)
        {
            var daysSinceMonday = ((int)utcNow.DayOfWeek + 6) % 7;
            return utcNow.Date.AddDays(-daysSinceMonday);
        }
    }
}
