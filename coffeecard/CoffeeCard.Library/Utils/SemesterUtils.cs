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
            if (currentTime.Month >= 7)
                return new DateTime(currentTime.Year, 7, 1);

            // Spring semester: Get last Monday of January.
            var lastDayOfJan = new DateTime(currentTime.Year, 1, 31);
            var correctedLastDayOfWeek = (int)(lastDayOfJan.DayOfWeek + 6) % 7; // Mon=0, Tue=1, ..., Sun=6
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
    }
}
