using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Services;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.User;
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
    [Route("api/v{version:apiVersion}/account")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ClaimsUtilities _claimsUtilities;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        public AccountController(IAccountService accountService, ClaimsUtilities claimsUtilities)
        {
            _accountService = accountService;
            _claimsUtilities = claimsUtilities;
        }

        /// <summary>
        /// Request the deletion of the user coupled to the provided token
        /// </summary>
        /// <response code="204">Successful initiation of account deletion process</response>
        /// <response code="404">Account not found</response>
        /// <response code="401">Invalid credentials</response>
        [HttpDelete]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiException), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult> Delete()
        {
            var user = await _claimsUtilities.ValidateAndReturnUserFromClaimAsync(User.Claims);
            
            await _accountService.RequestAnonymization(user);
            return NoContent();
        }
        
        /// <summary>
        /// Check if a given email is in use
        /// </summary>
        /// <returns>Account information</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPost]
        [ProducesResponseType(typeof(EmailInUseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        [Route("check-email-in-use")]
        public async Task<ActionResult<EmailInUseResponse>> EmailInUse([FromBody] EmailInUseRequest request)
        {
            var emailInUse = await _accountService.EmailExists(request.Email);
            return Ok(new EmailInUseResponse()
            {
                InUse = emailInUse
            });
        }
    }
}