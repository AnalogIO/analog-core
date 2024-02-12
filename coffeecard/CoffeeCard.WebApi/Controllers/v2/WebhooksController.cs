using CoffeeCard.Library.Services.v2;
using CoffeeCard.Models.DataTransferObjects.v2.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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

        public WebhooksController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPut("/accounts/user-group")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateUserGroupsAsync([FromBody][Required] WebhookUpdateUserGroupRequest request)
        {
            await _accountService.UpdatePriviligedUserGroups(request);
            return NoContent();
        }
    }
}
