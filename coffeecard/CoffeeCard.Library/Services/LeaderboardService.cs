using System;
using System.Collections.Generic;
using System.Linq;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Library.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly CoffeeCardContext _context;

        public LeaderboardService(CoffeeCardContext context)
        {
            _context = context;
        }

        public List<LeaderboardDto> GetLeaderboard(int preset, int top)
        {
            Func<Statistic, bool> filterExpression = preset switch
            {
                (int)StatisticPreset.Total => s => s.Preset == StatisticPreset.Total,
                (int)StatisticPreset.Semester => s => s.Preset == StatisticPreset.Semester &&
                                                      SemesterUtils.IsSwipeValidInSemester(s.LastSwipe, DateTime.Now),
                (int)StatisticPreset.Monthly => s => s.Preset == StatisticPreset.Monthly &&
                                                     SemesterUtils.IsSwipeValidInMonth(s.LastSwipe, DateTime.Now),
                _ => throw new ApiException("Not a correct preset was given", StatusCodes.Status400BadRequest)
            };

            var statistics = _context.Statistics
                .Include(s => s.User)
                .AsEnumerable()
                .Where(filterExpression)
                .OrderByDescending(s => s.SwipeCount)
                .ThenBy(s => s.LastSwipe)
                .AsEnumerable()
                .Take(top);
            
            return statistics.Select(s => new LeaderboardDto
                { 
                    Name = s.User.PrivacyActivated ? "Anonymous" : s.User.Name, 
                    Score = s.SwipeCount 
                }).ToList();
        }

        public (int Total, int Semester, int Month) GetLeaderboardPlacement(User user)
        {
            var leaderBoardPlacement = (Total: 0, Semester: 0, Month: 0);

            var totalUsers = _context.Statistics.Include(x => x.User).Where(s => s.Preset == StatisticPreset.Total)
                .OrderByDescending(x => x.SwipeCount).ThenBy(x => x.LastSwipe).ToList();
            var totalSwipeRank = totalUsers.FindIndex(x => x.User.Id == user.Id) + 1;

            var semesterUsers = _context.Statistics.Include(x => x.User)
                .AsEnumerable()
                .Where(s => s.Preset == StatisticPreset.Semester && SemesterUtils.IsSwipeValidInSemester(s.LastSwipe, DateTime.Now))
                .OrderByDescending(x => x.SwipeCount).ThenBy(x => x.LastSwipe).ToList();
            var semesterSwipeRank = semesterUsers.FindIndex(x => x.User.Id == user.Id) + 1;

            var monthUsers = _context.Statistics.Include(x => x.User)
                .AsEnumerable()
                .Where(s => s.Preset == StatisticPreset.Monthly && SemesterUtils.IsSwipeValidInMonth(s.LastSwipe, DateTime.Now))
                .OrderByDescending(x => x.SwipeCount).ThenBy(x => x.LastSwipe).ToList();
            var monthSwipeRank = monthUsers.FindIndex(x => x.User.Id == user.Id) + 1;

            leaderBoardPlacement.Total = totalSwipeRank;
            leaderBoardPlacement.Semester = semesterSwipeRank;
            leaderBoardPlacement.Month = monthSwipeRank;

            return leaderBoardPlacement;
        }
    }
}