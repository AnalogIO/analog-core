using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeCard.Library.Services.v2
{
    public class StatisticService : IStatisticService
    {
        private readonly CoffeeCardContext _context;

        public StatisticService(CoffeeCardContext context)
        {
            _context = context;
        }

        public async Task IncreaseStatisticsBy(int userId, int increaseBy)
        {
            User user = await _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Statistics)
                .Include(u => u.Tickets)
                .FirstOrDefaultAsync();

            DateTime utcNow = DateTime.UtcNow;

            // total statistics
            Statistic totalStatistics = user.Statistics.FirstOrDefault(x => x.Preset == StatisticPreset.Total);
            if (totalStatistics == null)
            {
                totalStatistics = new Statistic
                {
                    ExpiryDate = DateTime.UtcNow.AddYears(100),
                    Preset = StatisticPreset.Total,
                    SwipeCount = user.Tickets.Count(t => t.IsUsed) - increaseBy,
                    LastSwipe = utcNow
                };
                user.Statistics.Add(totalStatistics);
            }

            totalStatistics.SwipeCount += increaseBy;
            totalStatistics.LastSwipe = utcNow;

            // semester statistics
            DateTime semesterStart = SemesterUtils.GetSemesterStart(utcNow);
            DateTime semesterEnd = SemesterUtils.GetSemesterEnd(utcNow);
            Statistic semesterStatistics = user.Statistics.FirstOrDefault(x => x.Preset == StatisticPreset.Semester);

            if (semesterStatistics == null)
            {
                semesterStatistics = new Statistic
                {
                    ExpiryDate = semesterEnd,
                    Preset = StatisticPreset.Semester,
                    SwipeCount =
                        user.Tickets.Count(t => t.IsUsed && t.DateUsed > semesterStart && t.DateUsed < semesterEnd) -
                        increaseBy,
                    LastSwipe = utcNow
                };
                user.Statistics.Add(semesterStatistics);
            }
            else
            {
                // new semester has started
                if (semesterStatistics.ExpiryDate < semesterEnd)
                {
                    semesterStatistics.ExpiryDate = semesterEnd;
                    semesterStatistics.SwipeCount = 0;
                }
            }

            semesterStatistics.SwipeCount += increaseBy;
            semesterStatistics.LastSwipe = utcNow;


            // monthly statistics
            DateTime monthStart = new DateTime(utcNow.Year, utcNow.Month, 1);
            DateTime monthEnd = new DateTime(utcNow.Year, utcNow.Month, 1, 23, 59, 59).AddMonths(1).AddDays(-1);
            Statistic monthStatistics = user.Statistics.FirstOrDefault(x => x.Preset == StatisticPreset.Monthly);

            if (monthStatistics == null)
            {
                monthStatistics = new Statistic
                {
                    ExpiryDate = monthEnd,
                    Preset = StatisticPreset.Monthly,
                    SwipeCount = user.Tickets.Count(t => t.IsUsed && t.DateUsed > monthStart && t.DateUsed < monthEnd) -
                                 increaseBy,
                    LastSwipe = utcNow
                };
                user.Statistics.Add(monthStatistics);
            }
            else
            {
                // new month has started
                if (monthStatistics.ExpiryDate < monthEnd)
                {
                    monthStatistics.ExpiryDate = monthEnd;
                    monthStatistics.SwipeCount = 0;
                }
            }

            monthStatistics.SwipeCount += increaseBy;
            monthStatistics.LastSwipe = utcNow;
        }
    }
}