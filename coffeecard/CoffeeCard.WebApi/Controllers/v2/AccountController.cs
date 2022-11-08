using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Services;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects;
using CoffeeCard.Models.DataTransferObjects.v2.User;
using CoffeeCard.Models.DataTransferObjects.V2.User;
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
        /// Register data request. An account is required to verify its email before logging in
        /// </summary>
        /// <param name="registerRequest">Register data object</param>
        /// <response code="201">Successful account creation. Verification request email sent to provided email</response>
        /// <response code="409">Email already registered</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<MessageResponseDto>> Register([FromBody] RegisterAccountRequest registerRequest)
        {
            await _accountService.RegisterAccountAsync(registerRequest.Name, registerRequest.Email, registerRequest.Password, registerRequest.ProgrammeId);

            return Created("/api/v1/account/Get", new MessageResponseDto()
            {
                Message =
                    "Your user has been created! Please check your email to verify your account.\n(Check your spam folder!)"
            });
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
        /// <param name="request">The email that should be checked</param>
        /// <returns>Account information</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPost]
        [ProducesResponseType(typeof(EmailExistsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        [Route("email-exists")]
        public async Task<ActionResult<EmailExistsResponse>> EmailExists([FromBody] EmailExistsRequest request)
        {
            var emailInUse = await _accountService.EmailExists(request.Email);
            return Ok(new EmailExistsResponse()
            {
                EmailExists = emailInUse
            });
        }
    }
}