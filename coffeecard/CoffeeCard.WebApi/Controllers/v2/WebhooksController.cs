using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Models.DataTransferObjects.v2.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers.v2
{
    /// <summary>
    /// Controller for creating and managing user accounts
    /// </summary>
    [ApiController]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/webhooks")]
    [Authorize(AuthenticationSchemes = "apikey")]
    public class WebhooksController : ControllerBase
    {
        private readonly IAccountService _accountService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebhooksController"/> class.
        /// </summary>
        public WebhooksController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Update user groups in bulk
        /// </summary>
        /// <param name="request">The request containing the new user groups</param>
        /// <returns>No content</returns>
        /// <response code="204">The user groups were updated</response>
        /// <response code="400">Bad request. See explanation</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPut("accounts/user-group")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUserGroupsAsync(
            [FromBody] [Required] WebhookUpdateUserGroupRequest request
        )
        {
            await _accountService.UpdatePriviligedUserGroups(request);
            return NoContent();
        }
    }
}
