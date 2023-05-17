using System;
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
            var statPreset = preset.ToStatisticPreset();
            var (start, end) = PresetStartAndEnd(statPreset);

            var sortedStatistics = _context.Statistics
                .Where(s => s.Preset == statPreset)
                .Where(s => start < s.LastSwipe && s.LastSwipe < end)
                .OrderByDescending(s => s.SwipeCount)
                .ThenBy(s => s.LastSwipe)
                .Take(top)
                .Include(s => s.User)
                .AsEnumerable();

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
            var statPreset = preset.ToStatisticPreset();
            var (start, end) = PresetStartAndEnd(statPreset);

            var sortedStatistics = await _context.Statistics
                .Where(s => s.Preset == statPreset)
                .Where(s => start < s.LastSwipe && s.LastSwipe < end)
                .OrderByDescending(s => s.SwipeCount)
                .ThenBy(s => s.LastSwipe)
                .ToListAsync();

            var rank = sortedStatistics
                .FindIndex(s => s.UserId == user.Id) + 1;
            var userStatistic = sortedStatistics
                .FirstOrDefault(s => s.UserId == user.Id);

            var swipeCount = userStatistic?.SwipeCount ?? 0;

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
            var leaderBoardPlacement = (Total: 0, Semester: 0, Month: 0);

            var totalSwipe = await GetLeaderboardEntry(user, LeaderboardPreset.Total);

            var semesterSwipe = await GetLeaderboardEntry(user, LeaderboardPreset.Semester);

            var monthSwipe = await GetLeaderboardEntry(user, LeaderboardPreset.Month);

            leaderBoardPlacement.Total = totalSwipe.Rank;
            leaderBoardPlacement.Semester = semesterSwipe.Rank;
            leaderBoardPlacement.Month = monthSwipe.Rank;

            return leaderBoardPlacement;
        }
        

        private (DateTime, DateTime) PresetStartAndEnd(StatisticPreset preset)
        {
            var now = _dateTimeProvider.UtcNow();

            return preset switch
            {
                StatisticPreset.Semester => SemesterUtils.GetSemesterStartAndEnd(now),
                StatisticPreset.Monthly => SemesterUtils.GetMonthStartAndEnd(now),
                _ => (DateTime.UnixEpoch, DateTime.UtcNow.AddDays(1))// For StatisticPreset.Total a swipe does not expire
            };
        }
    }
}