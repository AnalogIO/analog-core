using System;

namespace CoffeeCard.Library.Utils
{
    public static class SemesterUtils
    {
        /// <summary>
        /// Get the start of date of the semester.
        /// When <paramref name="currentTime"/> is in July (Month 7) or greater, the semester start is July 1st or when earlier than July, the semester start is the date of the last Monday of January.
        /// </summary>
        public static DateTime GetSemesterStart(DateTime currentTime)
        {
            // Autumn semester: Get first day of July.
            if (currentTime.Month >= 7) return new DateTime(currentTime.Year, 7, 1);

            // Spring semester: Get last Monday of January.
            var lastDayOfJan = new DateTime(currentTime.Year, 1, 31);
            var correctedLastDayOfWeek = (int) (lastDayOfJan.DayOfWeek + 6) % 7; // Mon=0, Tue=1, ..., Sun=6
            return lastDayOfJan.AddDays(-correctedLastDayOfWeek);
        }

        /// <summary>
        /// Get the end of date of the semester.
        /// When <paramref name="currentTime"/> is in June (Month 6) or earlier, the semester end is June 30th or when later semester end is December 23rd.
        /// </summary>
        public static DateTime GetSemesterEnd(DateTime currentTime)
        {
            return currentTime.Month < 7
                ? new DateTime(currentTime.Year, 6, 30)
                : new DateTime(currentTime.Year, 12, 23);
        }

        /// <summary>
        /// Validate if swipe is expired compared by current month and year
        /// </summary>
        /// <param name="lastSwipe">Last swipe</param>
        /// <param name="now">Current DateTime</param>
        /// <returns>Valid if the year and month of lastSwipe is equal to now's year and month, else expired</returns>
        public static (DateTime, DateTime) GetMonthStartAndEnd(DateTime now)
        {
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return (startDate, endDate);
        }

        /// <summary>
        /// Validate if swipe is expired compared by semester start and semester end
        /// </summary>
        /// <param name="lastSwipe">Last swipe</param>
        /// <param name="now">Current DateTime</param>
        /// <returns>Valid if last swipe is equal to or within semester start and end, else expired</returns>
        public static (DateTime, DateTime) GetSemesterStartAndEnd(DateTime now)
        {
            return (GetSemesterStart(now), GetSemesterEnd(now));
        }
    }
}