using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CoffeeCard.Models
{
    public class User
    {
        public User()
        {
            Statistics = new List<Statistic>();
            Purchases = new List<Purchase>();
            Tickets = new List<Ticket>();
            Tokens = new List<Token>();
            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public int Experience { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public virtual ICollection<LoginAttempt> LoginAttempts { get; set; }
        public virtual ICollection<Token> Tokens { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
        public virtual ICollection<Statistic> Statistics { get; set; }
        public bool IsVerified { get; set; }
        public bool PrivacyActivated { get; set; }

        [ForeignKey("Programme_Id")] public virtual Programme Programme { get; set; }

        public int CalculateLevelFromXp()
        {
            var level = Math.Floor(1 + 1.0 / 10 * (Math.Sqrt(4 * Experience + 25) - 5));
            return Convert.ToInt32(level);
        }

        public int CalculateRequiredXpByLevel()
        {
            var lvl = CalculateLevelFromXp();
            var xp = Convert.ToInt32(25 * Math.Pow(lvl, 2) + 25 * lvl);
            return xp;
        }

        public void IncreaseStatisticsBy(int increaseBy)
        {
            var utcNow = DateTime.UtcNow;

            // total statistics
            var totalStatistics = Statistics.FirstOrDefault(x => x.Preset == StatisticPreset.Total);
            if (totalStatistics == null)
            {
                totalStatistics = new Statistic
                {
                    ExpiryDate = DateTime.UtcNow.AddYears(100), Preset = StatisticPreset.Total,
                    SwipeCount = Tickets.Count(t => t.IsUsed) - increaseBy, LastSwipe = utcNow
                };
                Statistics.Add(totalStatistics);
            }

            totalStatistics.SwipeCount += increaseBy;
            totalStatistics.LastSwipe = utcNow;

            // semester statistics
            var semesterStart = Statistic.GetSemesterStart(utcNow);
            var semesterEnd = Statistic.GetSemesterEnd(utcNow);
            var semesterStatistics = Statistics.FirstOrDefault(x => x.Preset == StatisticPreset.Semester);

            if (semesterStatistics == null)
            {
                semesterStatistics = new Statistic
                {
                    ExpiryDate = semesterEnd, Preset = StatisticPreset.Semester,
                    SwipeCount =
                        Tickets.Count(t => t.IsUsed && t.DateUsed > semesterStart && t.DateUsed < semesterEnd) -
                        increaseBy,
                    LastSwipe = utcNow
                };
                Statistics.Add(semesterStatistics);
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
            var monthStart = new DateTime(utcNow.Year, utcNow.Month, 1);
            var monthEnd = new DateTime(utcNow.Year, utcNow.Month, 1, 23, 59, 59).AddMonths(1).AddDays(-1);
            var monthStatistics = Statistics.FirstOrDefault(x => x.Preset == StatisticPreset.Monthly);

            if (monthStatistics == null)
            {
                monthStatistics = new Statistic
                {
                    ExpiryDate = monthEnd, Preset = StatisticPreset.Monthly,
                    SwipeCount = Tickets.Count(t => t.IsUsed && t.DateUsed > monthStart && t.DateUsed < monthEnd) -
                                 increaseBy,
                    LastSwipe = utcNow
                };
                Statistics.Add(monthStatistics);
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