using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.Leaderboard;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2
{
    public interface ILeaderboardService
    {
        /// <summary>
        /// Get top leaderboard entries filtered by preset
        /// </summary>
        /// <param name="preset">Preset to filter on</param>
        /// <param name="top">Value of the top results which should be returned</param>
        /// <returns>List of leaderboard entries</returns>
        Task<IEnumerable<LeaderboardEntry>> GetTopLeaderboardEntries(LeaderboardPreset preset, int top);

        /// <summary>
        /// Get leaderboard entry for user
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="preset">Preset to filter</param>
        /// <returns>Leaderboard entry for user</returns>
        Task<LeaderboardEntry> GetLeaderboardEntry(User user, LeaderboardPreset preset);
    }
}