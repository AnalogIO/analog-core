using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.v2.Leaderboard;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ClaimsUtilities _claimsUtilities;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeaderboardController"/> class.
        /// </summary>
        public LeaderboardController(
            ILeaderboardService leaderboardService,
            ClaimsUtilities claimsUtilities
        )
        {
            _leaderboardService = leaderboardService;
            _claimsUtilities = claimsUtilities;
        }

        /// <summary>
        /// Gets the top leaderboard by the specified preset
        /// </summary>
        /// <param name="preset">Leaderboard preset for date filtering. See <c>LeaderboardPreset</c> for possible values</param>
        /// <param name="top">Number of top results to return</param>
        /// <returns>Top leader board entries</returns>
        /// <response code="200">Top leader board entries</response>
        [HttpGet("top")]
        [ProducesResponseType(typeof(List<LeaderboardEntry>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LeaderboardEntry>>> GetTopEntries(
            [FromQuery] LeaderboardPreset preset = LeaderboardPreset.Semester,
            [FromQuery] int top = 10
        )
        {
            return Ok(await _leaderboardService.GetTopLeaderboardEntries(preset, top));
        }

        /// <summary>
        /// Get leaderboard stats for authenticated user. A user will have rank 0 if they do not have any valid swipes
        /// </summary>
        /// <param name="preset">Leaderboard preset for date filtering. See <c>LeaderboardPreset</c> for possible values</param>
        /// <returns>Leader board entry for user</returns>
        /// <response code="200">Leaderboard entry for user</response>
        /// <response code="401">Invalid credentials</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(LeaderboardEntry), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LeaderboardEntry>> Get(
            [FromQuery] LeaderboardPreset preset = LeaderboardPreset.Semester
        )
        {
            var user = await _claimsUtilities.ValidateAndReturnUserFromClaimAsync(User.Claims);

            return Ok(await _leaderboardService.GetLeaderboardEntry(user, preset));
        }
    }
}
