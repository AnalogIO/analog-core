using System.Collections.Generic;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Models.DataTransferObjects.v2.Leaderboard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers.v2
{
    /// <summary>
    /// Controller for retrieving the leaderboard
    /// </summary>
    [ApiController]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/leaderboard")]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeaderboardController"/> class.
        /// </summary>
        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        /// <summary>
        /// Gets the leaderboard by the specified preset
        /// </summary>
        /// <param name="preset">Leaderboard preset for date filtering</param>
        /// <param name="top">Number of results to return</param>
        /// <returns>Top leader board users</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<LeaderboardEntry>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public ActionResult<List<LeaderboardEntry>> Get([FromQuery] LeaderboardPreset preset = LeaderboardPreset.Semester, [FromQuery] int top = 10)
        {
            return Ok(_leaderboardService.GetLeaderboard(preset, top));
        }
    }
}