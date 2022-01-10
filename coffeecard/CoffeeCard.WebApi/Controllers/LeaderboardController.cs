using System.Collections.Generic;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.DataTransferObjects;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers
{
    /// <summary>
    /// Controller for retrieving the leaderboard
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
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
        /// <param name="preset">Leaderboard preset. 0 - Monthly, 1 - Semester and 2 - Total</param>
        /// <param name="top">Number of results to return</param>
        /// <returns>Top leader board users</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<LeaderboardDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public ActionResult<List<LeaderboardDto>> Get([FromQuery] int preset, [FromQuery] int top = 10)
        {
            var leaderboard = _leaderboardService.GetLeaderboard(preset, top);
            return Ok(leaderboard);
        }
    }
}