using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects;
using CoffeeCard.Models.DataTransferObjects.User;
using CoffeeCard.Models.DataTransferObjects.v2.Programme;
using CoffeeCard.Models.DataTransferObjects.v2.Token;
using CoffeeCard.Models.DataTransferObjects.v2.User;
using CoffeeCard.Models.Entities;
using CoffeeCard.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
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
        private readonly ILeaderboardService _leaderboardService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        public AccountController(
            IAccountService accountService,
            ClaimsUtilities claimsUtilities,
            ILeaderboardService leaderboardService
        )
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
        public async Task<ActionResult<MessageResponseDto>> Register(
            [FromBody] RegisterAccountRequest registerRequest
        )
        {
            await _accountService.RegisterAccountAsync(
                registerRequest.Name,
                registerRequest.Email,
                registerRequest.Password,
                registerRequest.ProgrammeId
            );

            return Created(
                "/api/v2/account/Get",
                new MessageResponseDto
                {
                    Message =
                        "Your user has been created! Please check your email to verify your account.\n(Check your spam folder!)",
                }
            );
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
        public async Task<ActionResult<UserResponse>> Update(
            [FromBody] UpdateUserRequest updateUserRequest
        )
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
        public async Task<ActionResult<EmailExistsResponse>> EmailExists(
            [FromBody] EmailExistsRequest request
        )
        {
            var emailInUse = await _accountService.EmailExistsAsync(request.Email);
            return Ok(new EmailExistsResponse { EmailExists = emailInUse });
        }

        /// <summary>
        /// Updates the user group of a user
        /// </summary>
        /// <param name="id"> id of the user whose userGroup will be updated </param>
        /// <param name="updateUserGroupRequest"> Update User Group information request  </param>
        /// <returns> no content result </returns>
        /// <response code="204"> The update was processed </response>
        /// <response code="401"> Invalid credentials </response>
        /// <response code="404"> User not found </response>
        [HttpPatch]
        [AuthorizeRoles(UserGroup.Board)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [Route("{id:int}/user-group")]
        public async Task<ActionResult> UpdateAccountUserGroup(
            [FromRoute] int id,
            [FromBody] UpdateUserGroupRequest updateUserGroupRequest
        )
        {
            await _accountService.UpdateUserGroup(updateUserGroupRequest.UserGroup, id);

            return new NoContentResult();
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
            [FromBody] ResendAccountVerificationEmailRequest request
        )
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
                    FullName = user.Programme.FullName,
                },
                PrivacyActivated = user.PrivacyActivated,
            };
        }

        /// <summary>
        /// Searches a user in the database
        /// </summary>
        /// <param name="filter">A filter to search by Id, Name or Email. When an empty string is given, all users will be returned</param>
        /// <param name="pageNum">The page number</param>
        /// <param name="pageLength">The length of a page</param>
        /// <returns> A collection of User objects that match the search criteria </returns>
        /// <response code="200">Users, possible with filter applied</response>
        /// <response code="401"> Invalid credentials </response>
        [HttpGet]
        [AuthorizeRoles(UserGroup.Board)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(UserSearchResponse), StatusCodes.Status200OK)]
        [Route("search")]
        public async Task<ActionResult<UserSearchResponse>> SearchUsers(
            [FromQuery] [Range(0, int.MaxValue)] int pageNum,
            [FromQuery] string filter = "",
            [FromQuery] [Range(1, 100)] int pageLength = 30
        )
        {
            return Ok(await _accountService.SearchUsers(filter, pageNum, pageLength));
        }

        /// <summary>
        /// Sends a magic link to the user's email to login
        /// </summary>
        /// <param name="request">User's email</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Login([FromBody] UserLoginRequest request)
        {
            await _accountService.SendMagicLinkEmail(request.Email, request.LoginType);
            return new NoContentResult();
        }

        /// <summary>
        /// Authenticates the user with the token hash from a magic link
        /// </summary>
        /// <param name="token">The token hash from the magic link</param>
        /// <returns>A JSON Web Token used to authenticate for other endpoints and a refresh token to re-authenticate without a new magic link</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("auth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<UserLoginResponse>> Authenticate(TokenLoginRequest token)
        {
            if (token is null)
                return NotFound(
                    new MessageResponseDto { Message = "Token required for app authentication." }
                );

            var userTokens = await _accountService.GenerateUserLoginFromToken(token);
            return Ok(userTokens);
        }
    }
}
