using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects;
using CoffeeCard.Models.DataTransferObjects.User;
using System.Linq;
using System.Security.Claims;

namespace CoffeeCard.WebApi.Controllers
{
    /// <summary>
    /// Controller for creating and managing user accounts
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILeaderboardService _leaderboardService;
        private readonly IMapperService _mapperService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        public AccountController(IAccountService accountService, ILeaderboardService leaderboardService,
            IMapperService mapperService, IHttpContextAccessor httpContextAccessor)
        {
            _accountService = accountService;
            _leaderboardService = leaderboardService;
            _mapperService = mapperService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Register a new account. A account is required to verify its email before logging in
        /// </summary>
        /// <param name="registerDto">Register data object</param>
        /// <response code="201">Successful account creation</response>
        /// <response code="409">Email already registered</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<MessageResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            await _accountService.RegisterAccountAsync(registerDto.Name, registerDto.Email, registerDto.Password);
            return Created(nameof(Get), new MessageResponseDto()
            {
                Message =
                    "Your user has been created! Please check your email to verify your account.\n(Check your spam folder!)"
            });
        }

        /// <summary>
        /// Returns a token that is used to identify the account
        /// </summary>
        /// <returns>Login token</returns>
        /// <param name="loginDto">Login data object</param>
        /// <response code="200">Successful account login</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="403">Account email not verified</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status429TooManyRequests)]
        public ActionResult<TokenDto> Login([FromBody] LoginDto loginDto)
        {
            var token = _accountService.Login(loginDto.Email, loginDto.Password, loginDto.Version);
            if (token == null)
            {
                Log.Information("Unsuccessful login for e-mail = {Email} from IP = {Ipaddress} ", loginDto.Email,
                    _httpContextAccessor.HttpContext.Connection.RemoteIpAddress);
                return Unauthorized();
            }

            return Ok(new TokenDto { Token = token });
        }

        /// <summary>
        /// Returns basic data about the account
        /// </summary>
        /// <returns>Account information</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpGet]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public ActionResult<UserDto> Get()
        {
            var user = _accountService.GetAccountByClaims(User.Claims);
            var userDto = _mapperService.Map(user);
            var leaderBoardPlacement = _leaderboardService.GetLeaderboardPlacement(user);
            userDto.RankAllTime = leaderBoardPlacement.Total;
            userDto.RankSemester = leaderBoardPlacement.Semester;
            userDto.RankMonth = leaderBoardPlacement.Month;
            userDto.Role = user.UserGroup;
            return Ok(userDto);
        }

        /// <summary>
        /// Updates the account and returns the updated values.
        /// Only properties which are present in the <see cref="UpdateUserDto"/> will be updated
        /// </summary>
        /// <param name="updateUserDto">Update account information request. All properties are optional as the server only
        /// updates the values of the properties which are present</param>
        /// <returns>Account information</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPut]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public ActionResult<UserDto> Update([FromBody] UpdateUserDto updateUserDto)
        {
            var user = _accountService.UpdateAccount(User.Claims, updateUserDto);
            var leaderBoardPlacement = _leaderboardService.GetLeaderboardPlacement(user);
            var userDto = _mapperService.Map(user);
            userDto.RankAllTime = leaderBoardPlacement.Total;
            userDto.RankSemester = leaderBoardPlacement.Semester;
            userDto.RankMonth = leaderBoardPlacement.Month;
            return Ok(userDto);
        }

        /// <summary>
        /// </summary>
        /// <param name="emailDTO">Account email</param>
        /// <returns>Account information</returns>
        /// <response code="200">Successful request</response>
        /// <response code="404">E-mail not found</response>
        [HttpPost("forgotpassword")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] EmailDto emailDTO)
        {
            await _accountService.ForgotPasswordAsync(emailDTO.Email);

            Log.Information("Password reset requested for e-mail = {Email} from IP = {Ipaddress}", emailDTO.Email,
                _httpContextAccessor.HttpContext.Connection.RemoteIpAddress);

            return Ok(new MessageResponseDto()
            {
                Message = "We have send you a confirmation email"
            });
        }
    }
}