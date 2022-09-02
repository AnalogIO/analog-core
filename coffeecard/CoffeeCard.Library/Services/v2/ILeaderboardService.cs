using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.Leaderboard;

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
        Task<List<LeaderboardEntry>> GetLeaderboard(LeaderboardPreset preset, int top);
    }
}