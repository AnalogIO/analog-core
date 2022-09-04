using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.v2.Leaderboard;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Library.Services.v2
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly CoffeeCardContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public LeaderboardService(CoffeeCardContext context, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IEnumerable<LeaderboardEntry>> GetTopLeaderboardEntries(LeaderboardPreset preset, int top)
        {
            var allStatisticsByPreset = await _context.Statistics
                .Include(s => s.User)
                .Where(s => s.Preset == preset.ToStatisticPreset())
                .ToListAsync();

            var topStatistics = allStatisticsByPreset
                .Where(IsSwipeValid)
                .OrderByDescending(s => s.SwipeCount)
                .ThenBy(s => s.LastSwipe)
                .Take(top);

            return topStatistics.Select((s, index) => new LeaderboardEntry
            {
                Id = s.User.Id,
                Name = s.User.PrivacyActivated ? "Anonymous" : s.User.Name,
                Rank = index + 1,
                Score = s.SwipeCount
            });
        }

        public async Task<LeaderboardEntry> GetLeaderboardEntry(User user, LeaderboardPreset preset)
        {
            var allStatisticsByPreset = await _context.Statistics
                .Where(s => s.Preset == preset.ToStatisticPreset())
                .ToListAsync();

            var sortedStatistics = allStatisticsByPreset
                .Where(IsSwipeValid)
                .OrderByDescending(s => s.SwipeCount)
                .ThenBy(s => s.LastSwipe)
                .ToList();

            var rank = sortedStatistics
                .FindIndex(s => s.User.Id == user.Id) + 1;
            var userStatistic = sortedStatistics
                .FirstOrDefault(s => s.User.Id == user.Id);

            var swipeCount = 0;
            if (userStatistic != null)
            {
                swipeCount = userStatistic.SwipeCount;
            }

            return new LeaderboardEntry
            {
                Id = user.Id,
                Name = user.Name,
                Rank = rank,
                Score = swipeCount
            };
        }

        private bool IsSwipeValid(Statistic s)
        {
            var now = _dateTimeProvider.UtcNow();

            return s.Preset switch
            {
                StatisticPreset.Semester => SemesterUtils.IsSwipeValidInSemester(s.LastSwipe, now),
                StatisticPreset.Monthly => SemesterUtils.IsSwipeValidInMonth(s.LastSwipe, now),
                _ => true // For StatisticPreset.Total a swipe does not expire
            };
        }
    }
}