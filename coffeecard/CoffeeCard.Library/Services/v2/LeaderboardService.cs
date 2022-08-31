using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.v2.Leaderboard;
using Microsoft.EntityFrameworkCore;
using static System.Linq.Queryable;

namespace CoffeeCard.Library.Services.v2
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly CoffeeCardContext _context;

        public LeaderboardService(CoffeeCardContext context)
        {
            _context = context;
        }

        public Task<List<LeaderboardEntry>> GetLeaderboard(LeaderboardPreset preset, int top)
        {
            var statistics = _context.Statistics
                .Include(s => s.User)
                .Where(s => s.Preset == preset.ToStatisticPreset())
                .OrderByDescending(s => s.SwipeCount)
                .ThenBy(s => s.LastSwipe)
                .Take(top);

            return statistics.Select(s => new LeaderboardEntry
            {
                Id = s.User.Id,
                Name = s.User.PrivacyActivated ? "Anonymous" : s.User.Name,
                Score = s.SwipeCount
            }).ToListAsync();
        }
    }
}