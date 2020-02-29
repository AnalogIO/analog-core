using System.Collections.Generic;
using CoffeeCard.WebApi.Models;

namespace CoffeeCard.WebApi.Services
{
    public interface ILeaderboardService
    {
        List<LeaderboardUser> GetLeaderboard(int preset, int top);
        (int Total, int Semester, int Month) GetLeaderboardPlacement(User user);
    }
}