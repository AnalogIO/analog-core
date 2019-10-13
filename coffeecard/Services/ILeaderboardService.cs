using coffeecard.Models;
using Coffeecard.Models;
using System.Collections.Generic;

namespace coffeecard.Services
{
    public interface ILeaderboardService
    {
        List<LeaderboardUser> GetLeaderboard(int preset, int top);
        (int Total, int Semester, int Month) GetLeaderboardPlacement(User user);
    }
}
