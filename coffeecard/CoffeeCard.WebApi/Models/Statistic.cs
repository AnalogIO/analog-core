using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeCard.WebApi.Models
{

    /**
    * This class will be used to optimize the queries to get statistics.
    * The preset defines the kind of statistic entity and each preset has a corresponding `SwipeCount`, `SwipeRank` and `ExpiryDate`.
    * This means that each user should have exactly 3 entries in the database after 1 or more swipes.
    * Each preset should be updated accordingly to the current date and the ExpiryDate. If the ExpiryDate of a entry is exceeded, then the `SwipeCount`, `SwipeRank` and `ExpiryDate` should be reset.
    **/
    public enum StatisticPreset { Monthly, Semester, Total }

    public class Statistic
    {
        public int Id { get; set; }
        public StatisticPreset Preset { get; set; }
        public int SwipeCount { get; set; }
        public DateTime LastSwipe { get; set; }
        public DateTime ExpiryDate { get; set; }

        [ForeignKey("User_Id")]
        public virtual User User { get; set; }

        public static DateTime GetSemesterStart(DateTime currentTime)
        {
            if (currentTime.Month < 7)
            {
                var jan1 = new DateTime(currentTime.Year, 1, 1);
                var dayOffset = DayOfWeek.Monday - jan1.DayOfWeek;
                var startDate = 29 - dayOffset;

                return new DateTime(currentTime.Year, 1, startDate);
            }

            return new DateTime(currentTime.Year, 7, 1);
        }

        public static DateTime GetSemesterEnd(DateTime currentTime)
        {
            if (currentTime.Month < 7) return new DateTime(currentTime.Year, 6, 30);

            return new DateTime(currentTime.Year, 12, 23);
        }

        public static bool ValidateMonthlyExpired(DateTime lastSwipe)
        {
            var now = DateTime.Now;
            if (lastSwipe.Month != now.Month) return false;
            return true;
        }

        public static bool ValidateSemesterExpired(DateTime lastSwipe)
        {
            var now = DateTime.Now;
            if (lastSwipe < GetSemesterStart(now)) return false;
            return true;
        }

        public static DateTime[] PresetToDateArr(string preset)
        {
            if (preset.Equals("semester"))
            {
                var semesterStart = GetSemesterStart(DateTime.UtcNow);
                var semesterEnd = GetSemesterEnd(DateTime.UtcNow);
                return new[] { semesterStart, semesterEnd };
            }

            if (preset.Equals("month"))
            {
                var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                var monthEnd = monthStart.AddMonths(1);
                return new[] { monthStart, monthEnd };
            }

            return new DateTime[0];
        }
    }
}