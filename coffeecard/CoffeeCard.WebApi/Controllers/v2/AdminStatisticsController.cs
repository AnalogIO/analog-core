using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Models.DataTransferObjects.v2.AdminStatistics;
using CoffeeCard.Models.Entities;
using CoffeeCard.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers.v2
{
    /// <summary>
    /// Controller for creating statistics endpoints
    /// </summary>
    [ApiController]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/statistics")]
    [Authorize]
    public class AdminStatisticsController : ControllerBase
    {
        private readonly IAdminStatisticsService _adminStatisticsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminStatisticsController"/> class.
        /// </summary>
        public AdminStatisticsController(IAdminStatisticsService adminStatisticsService)
        {
            _adminStatisticsService = adminStatisticsService;
        }

        /// <summary>
        /// Sum unused clip cards within a given period per productId
        /// </summary>
        /// <param name="unusedClipsRequest"> Request object containing start and end date of the query</param>
        /// <returns> A collection of UnusedClipsResponse objects that match the search criteria </returns>
        /// <response code="200"> Products with tickets that match the criteria </response>
        /// <response code="401"> Invalid credentials </response>
        [HttpPost("unused-clips")]
        [AuthorizeRoles(UserGroup.Board)]
        [ProducesResponseType(typeof(IEnumerable<UnusedClipsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<UnusedClipsResponse>>> GetUnusedClips(
            [FromBody] UnusedClipsRequest unusedClipsRequest
        )
        {
            var tickets = await _adminStatisticsService.GetUsableClips(unusedClipsRequest);
            return Ok(tickets);
        }
    }
}
