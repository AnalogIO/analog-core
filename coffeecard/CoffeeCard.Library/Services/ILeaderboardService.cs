using CoffeeCard.Models.DataTransferObjects;
using CoffeeCard.Models.Entities;
using System.Collections.Generic;

namespace CoffeeCard.Library.Services
{
    public interface ILeaderboardService
    {
        List<LeaderboardDto> GetLeaderboard(int preset, int top);
        (int Total, int Semester, int Month) GetLeaderboardPlacement(User user);
    }
}