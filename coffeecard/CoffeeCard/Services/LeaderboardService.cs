using System.Collections.Generic;
using System.Linq;
using CoffeeCard.Helpers;
using CoffeeCard.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly CoffeeCardContext _context;

        public LeaderboardService(CoffeeCardContext context)
        {
            _context = context;
        }

        public List<LeaderboardUser> GetLeaderboard(int preset, int top)
        {
            if (preset == (int) StatisticPreset.Total)
            {
                var users = _context.Statistics.Include(x => x.User).Where(s => s.Preset == StatisticPreset.Total)
                    .OrderByDescending(x => x.SwipeCount).ThenBy(x => x.LastSwipe).ToList().Take(top);
                return users.Select(s => new LeaderboardUser {Name = s.User.Name, Score = s.SwipeCount}).ToList();
            }

            if (preset == (int) StatisticPreset.Semester)
            {
                var users = _context.Statistics.Include(x => x.User)
                    .Where(s => s.Preset == StatisticPreset.Semester && Statistic.ValidateSemesterExpired(s.LastSwipe))
                    .OrderByDescending(x => x.SwipeCount).ThenBy(x => x.LastSwipe).ToList().Take(top);
                return users.Select(s => new LeaderboardUser {Name = s.User.Name, Score = s.SwipeCount}).ToList();
            }

            if (preset == (int) StatisticPreset.Monthly)
            {
                var users = _context.Statistics.Include(x => x.User)
                    .Where(s => s.Preset == StatisticPreset.Monthly && Statistic.ValidateMonthlyExpired(s.LastSwipe))
                    .OrderByDescending(x => x.SwipeCount).ThenBy(x => x.LastSwipe).ToList().Take(top);
                return users.Select(s => new LeaderboardUser
                    {Name = s.User.PrivacyActivated ? "Anonymous" : s.User.Name, Score = s.SwipeCount}).ToList();
            }

            throw new ApiException("Not a correct preset was given", 400);
        }

        public (int Total, int Semester, int Month) GetLeaderboardPlacement(User user)
        {
            var leaderBoardPlacement = (Total: 0, Semester: 0, Month: 0);

            var totalUsers = _context.Statistics.Include(x => x.User).Where(s => s.Preset == StatisticPreset.Total)
                .OrderByDescending(x => x.SwipeCount).ThenBy(x => x.LastSwipe).ToList();
            var totalSwipeRank = totalUsers.FindIndex(x => x.User.Id == user.Id) + 1;

            var semesterUsers = _context.Statistics.Include(x => x.User)
                .Where(s => s.Preset == StatisticPreset.Semester && Statistic.ValidateSemesterExpired(s.LastSwipe))
                .OrderByDescending(x => x.SwipeCount).ThenBy(x => x.LastSwipe).ToList();
            var semesterSwipeRank = semesterUsers.FindIndex(x => x.User.Id == user.Id) + 1;

            var monthUsers = _context.Statistics.Include(x => x.User)
                .Where(s => s.Preset == StatisticPreset.Monthly && Statistic.ValidateMonthlyExpired(s.LastSwipe))
                .OrderByDescending(x => x.SwipeCount).ThenBy(x => x.LastSwipe).ToList();
            var monthSwipeRank = monthUsers.FindIndex(x => x.User.Id == user.Id) + 1;

            leaderBoardPlacement.Total = totalSwipeRank;
            leaderBoardPlacement.Semester = semesterSwipeRank;
            leaderBoardPlacement.Month = monthSwipeRank;

            return leaderBoardPlacement;
        }
    }
}