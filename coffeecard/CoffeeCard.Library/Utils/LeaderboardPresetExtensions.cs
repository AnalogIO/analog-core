using CoffeeCard.Models.DataTransferObjects.v2.Leaderboard;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Utils
{
    public static class LeaderboardPresetExtensions
    {
        public static StatisticPreset ToStatisticPreset(this LeaderboardPreset preset)
        {
            return preset switch
            {
                LeaderboardPreset.Month => StatisticPreset.Monthly,
                LeaderboardPreset.Semester => StatisticPreset.Semester,
                LeaderboardPreset.Total => StatisticPreset.Total,
                _ => StatisticPreset.Monthly,
            };
        }
    }
}
