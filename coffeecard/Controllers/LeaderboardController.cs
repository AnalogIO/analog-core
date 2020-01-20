using System.Collections.Generic;
using CoffeeCard.Helpers;
using CoffeeCard.Models;
using CoffeeCard.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.Controllers
{

    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        ILeaderboardService _leaderboardService;
        IMapperService _mapperService;

        public LeaderboardController(ILeaderboardService leaderboardService, IMapperService mapperService)
        {
            _leaderboardService = leaderboardService;
            _mapperService = mapperService;
        }


        /// <summary>
        ///  Gets the highscore of the specified preset 0 - Monthly, 1 - Semester and 2 - Total
        /// </summary>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        public ActionResult<IEnumerable<LeaderboardUser>> Get(int preset, int top = 10)
        {
            var leaderboard = _leaderboardService.GetLeaderboard(preset, top);
            return leaderboard;
        }
    }
}
