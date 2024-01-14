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
        Task<IEnumerable<LeaderboardEntry>> GetTopLeaderboardEntries(
            LeaderboardPreset preset,
            int top
        );

        /// <summary>
        /// Get leaderboard entry for user. A user will have rank 0 if they do not have any valid swipes
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="preset">Preset to filter</param>
        /// <returns>Leaderboard entry for user</returns>
        Task<LeaderboardEntry> GetLeaderboardEntry(User user, LeaderboardPreset preset);

        /// <summary>
        /// Retruns a triple representing the users current placement on the leaderboard
        /// </summary>
        /// <param name="user">User</param>
        Task<(int Total, int Semester, int Month)> GetLeaderboardPlacement(User user);
    }
}
