using System;

namespace CoffeeCard.Library.Utils
{
    public static class StatisticUtils
    {
        public static int CalculateLevelFromXp(int userExperience)
        {
            var level = Math.Floor(1 + 1.0 / 10 * (Math.Sqrt(4 * userExperience + 25) - 5));
            return Convert.ToInt32(level);
        }

        public static int CalculateRequiredXpByLevel(int userExperience)
        {
            var lvl = CalculateLevelFromXp(userExperience);
            var xp = Convert.ToInt32(25 * Math.Pow(lvl, 2) + 25 * lvl);
            return xp;
        }
    }
}
