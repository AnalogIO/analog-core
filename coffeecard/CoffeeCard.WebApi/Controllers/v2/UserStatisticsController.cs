using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Models.DataTransferObjects.v2.UserStatistics;
using CoffeeCard.Models.Entities;
using CoffeeCard.WebApi.Helpers;
using Microsoft.AspNetCore.Http;

namespace CoffeeCard.WebApi.Controllers.v2
{
    /// <summary>
    /// Controller for creating statistics endpoints 
    /// </summary>
    [ApiController]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/statistics")]
    [Authorize]
    public class UserStatisticsController : ControllerBase
    {
        private readonly IUserStatisticsService _userStatisticsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserStatisticsController"/> class.
        /// </summary>
        public UserStatisticsController(IUserStatisticsService userStatisticsService)
        {
            _userStatisticsService = userStatisticsService;
        }

        /// <summary>
        /// Sum unused clip cards within a given period
        /// </summary>
        /// <param name="unusedClipsRequest"> Request object containing start and end date of the query</param>
        /// <returns> A collection of UnusedClipsResponse objects that match the search criteria </returns>
        /// <response code="200"> Tickets that match the criteria </response>
        /// <response code="401"> Invalid credentials </response>
        [HttpPost("unused-clips")]
        [AuthorizeRoles(UserGroup.Board)]
        [ProducesResponseType(typeof(UnusedClipsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UnusedClipsResponse>> GetUnusedClips([FromBody] UnusedClipsRequest unusedClipsRequest)
        {
            var tickets = await _userStatisticsService.GetUnusedClips(unusedClipsRequest);
            return Ok(tickets);
        }
    }
}