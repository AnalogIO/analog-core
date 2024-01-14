using System.Collections.Generic;
using CoffeeCard.Models.DataTransferObjects;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="LeaderboardController"/> class.
        /// </summary>
        public LeaderboardController() { }

        /// <summary>
        /// Gets the leaderboard by the specified preset
        /// </summary>
        /// <param name="preset">Leaderboard preset. 0 - Monthly, 1 - Semester and 2 - Total</param>
        /// <param name="top">Number of results to return</param>
        /// <returns>Top leader board users</returns>
        /// <response code="410">Deprecated</response>
        [HttpGet]
        [ProducesResponseType(typeof(void), StatusCodes.Status410Gone)]
        public ActionResult<List<LeaderboardDto>> Get(
            [FromQuery] int preset,
            [FromQuery] int top = 10
        )
        {
            return StatusCode(StatusCodes.Status410Gone);
        }
    }
}
