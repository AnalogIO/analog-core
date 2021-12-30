using System.Collections.Generic;
using CoffeeCard.Common.Models;

namespace CoffeeCard.Library.Services
{
    public interface ILeaderboardService
    {
        List<LeaderboardUser> GetLeaderboard(int preset, int top);
        (int Total, int Semester, int Month) GetLeaderboardPlacement(User user);
    }
}