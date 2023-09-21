using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects;
using CoffeeCard.Models.DataTransferObjects.v2.User;
using CoffeeCard.Models.DataTransferObjects.v2.Programme;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Models.Entities;
using Serilog;

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
        private readonly ILeaderboardService _leaderboardService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        public AccountController(IAccountService accountService, ClaimsUtilities claimsUtilities,
            ILeaderboardService leaderboardService)
        {
            _accountService = accountService;
            _claimsUtilities = claimsUtilities;
            _leaderboardService = leaderboardService;
        }

        /// <summary>
        /// Register data request. An account is required to verify its email before logging in
        /// </summary>
        /// <param name="registerRequest">Register data object</param>
        /// <response code="201">Successful account creation. Verification request email sent to provided email</response>
        /// <response code="409">Email already registered</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<MessageResponseDto>> Register([FromBody] RegisterAccountRequest registerRequest)
        {
            await _accountService.RegisterAccountAsync(registerRequest.Name, registerRequest.Email,
                registerRequest.Password, registerRequest.ProgrammeId);

            return Created("/api/v2/account/Get", new MessageResponseDto
            {
                Message =
                    "Your user has been created! Please check your email to verify your account.\n(Check your spam folder!)"
            });
        }

        /// <summary>
        /// Request the deletion of the user coupled to the provided token
        /// </summary>
        /// <response code="202">Successful initiation of account deletion process</response>
        /// <response code="404">Account not found</response>
        /// <response code="401">Invalid credentials</response>
        [HttpDelete]
        [ProducesResponseType(typeof(void), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult> Delete()
        {
            var user = await _claimsUtilities.ValidateAndReturnUserFromClaimAsync(User.Claims);
            await _accountService.RequestAnonymizationAsync(user);

            return StatusCode(StatusCodes.Status202Accepted);
        }


        /// <summary>
        /// Returns basic data about the account
        /// </summary>
        /// <returns>Account information</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpGet]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserResponse>> Get()
        {
            var user = await _claimsUtilities.ValidateAndReturnUserFromEmailClaimAsync(User.Claims);

            return Ok(await UserWithRanking(user));
        }

        /// <summary>
        /// Updates the account and returns the updated values.
        /// Only properties which are present in the <see cref="UpdateUserRequest"/> will be updated
        /// </summary>
        /// <param name="updateUserRequest">Update account information request. All properties are optional as the server only
        /// updates the values of the properties which are present</param>
        /// <returns>Account information</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPut]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserResponse>> Update([FromBody] UpdateUserRequest updateUserRequest)
        {
            var user = await _claimsUtilities.ValidateAndReturnUserFromEmailClaimAsync(User.Claims);
            var updatedUser = await _accountService.UpdateAccountAsync(user, updateUserRequest);

            return Ok(await UserWithRanking(updatedUser));
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
            var emailInUse = await _accountService.EmailExistsAsync(request.Email);
            return Ok(new EmailExistsResponse
            {
                EmailExists = emailInUse
            });
        }

        /// <summary>
        /// Resend account verification email if account is not already verified
        /// </summary>
        /// <param name="request">Email to be verified</param>
        /// <response code="200">Email has been sent</response>
        /// <response code="404">Email not found</response>
        /// <response code="409">Account already verified</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
        [Route("resend-verification-email")]
        public async Task<ActionResult> ResendVerificationEmail(
            [FromBody] ResendAccountVerificationEmailRequest request)
        {
            await _accountService.ResendAccountVerificationEmail(request);

            return Ok();
        }

        private async Task<UserResponse> UserWithRanking(User user)
        {
            var (total, semester, month) = await _leaderboardService.GetLeaderboardPlacement(user);

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                RankAllTime = total,
                RankMonth = month,
                RankSemester = semester,
                Name = user.Name,
                Role = user.UserGroup.toUserRole(),
                Programme = new ProgrammeResponse()
                {
                    Id = user.Programme.Id,
                    ShortName = user.Programme.ShortName,
                    FullName = user.Programme.FullName
                },
                PrivacyActivated = user.PrivacyActivated,
            };
        }
    }
}