using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.v2.Statistics;
using CoffeeCard.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers.v2
{
    /// <summary>
    /// Controller for public statistics endpoints.
    /// </summary>
    [ApiController]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/statistics")]
    [Authorize]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;
        private readonly ClaimsUtilities _claimsUtilities;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsController"/> class.
        /// </summary>
        public StatisticsController(
            IStatisticsService statisticsService,
            ClaimsUtilities claimsUtilities
        )
        {
            _statisticsService = statisticsService;
            _claimsUtilities = claimsUtilities;
        }

        /// <summary>
        /// Get some quick stats about the user and general drink consumption at ITU.
        /// </summary>
        /// <response code="200">Quick stats</response>
        /// <response code="401">Invalid credentials</response>
        [HttpGet("quick")]
        [ProducesResponseType(typeof(IEnumerable<QuickStatResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<QuickStatResponse>>> GetQuickStats()
        {
            var user = await _claimsUtilities.ValidateAndReturnUserFromClaimAsync(User.Claims);
            return Ok(await _statisticsService.GetQuickStatsAsync(user));
        }
    }
}
