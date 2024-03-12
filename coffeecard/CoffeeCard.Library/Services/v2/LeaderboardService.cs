using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.v2.Leaderboard;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            List<Statistic> sortedStatistics = await GetSortedStatistics(preset)
                .Take(top)
                .Include(s => s.User)
                .ToListAsync();

            return sortedStatistics.Select((s, index) => new LeaderboardEntry
            {
                Id = s.User.Id,
                Name = s.User.PrivacyActivated ? "Anonymous" : s.User.Name,
                Rank = index + 1,
                Score = s.SwipeCount
            });
        }

        public async Task<LeaderboardEntry> GetLeaderboardEntry(User user, LeaderboardPreset preset)
        {
            List<Statistic> sortedStatistics = await GetSortedStatistics(preset).ToListAsync();

            int rank = sortedStatistics
                .FindIndex(s => s.UserId == user.Id) + 1;
            Statistic userStatistic = sortedStatistics
                .FirstOrDefault(s => s.UserId == user.Id);

            int swipeCount = userStatistic?.SwipeCount ?? 0;

            return new LeaderboardEntry
            {
                Id = user.Id,
                Name = user.Name,
                Rank = rank,
                Score = swipeCount
            };
        }


        public async Task<(int Total, int Semester, int Month)> GetLeaderboardPlacement(User user)
        {
            (int Total, int Semester, int Month) leaderBoardPlacement = (Total: 0, Semester: 0, Month: 0);

            LeaderboardEntry totalSwipe = await GetLeaderboardEntry(user, LeaderboardPreset.Total);

            LeaderboardEntry semesterSwipe = await GetLeaderboardEntry(user, LeaderboardPreset.Semester);

            LeaderboardEntry monthSwipe = await GetLeaderboardEntry(user, LeaderboardPreset.Month);

            leaderBoardPlacement.Total = totalSwipe.Rank;
            leaderBoardPlacement.Semester = semesterSwipe.Rank;
            leaderBoardPlacement.Month = monthSwipe.Rank;

            return leaderBoardPlacement;
        }

        private IOrderedQueryable<Statistic> GetSortedStatistics(LeaderboardPreset preset)
        {
            StatisticPreset statPreset = preset.ToStatisticPreset();

            return _context.Statistics
                .Where(s => s.Preset == statPreset)
                .Where(s => _dateTimeProvider.UtcNow() <= s.ExpiryDate)
                .OrderByDescending(s => s.SwipeCount)
                .ThenBy(s => s.LastSwipe);
        }
    }
}