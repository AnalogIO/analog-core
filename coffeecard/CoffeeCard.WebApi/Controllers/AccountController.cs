using CoffeeCard.Common.Errors;
using CoffeeCard.Common.Models.DataTransferObjects.User;
using CoffeeCard.Library.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

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
        /// <response code="400">Invalid Account creation object</response>
        /// <response code="409">Email already registered</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiException), StatusCodes.Status409Conflict)]
        public IActionResult Register([FromBody] RegisterDto registerDto)
        {
            _accountService.RegisterAccount(registerDto);
            return Created("register", 
                new
                {
                    message =
                        "Your user has been created! Please check your email to verify your account.\n(Check your spam folder!)"
                });
        }

        /// <summary>
        /// Returns a token that is used to identify the account
        /// </summary>
        /// <returns>Login token</returns>
        /// <param name="loginDto">Login data object</param>
        /// <response code="200">Successful account login</response>
        /// <response code="400">Invalid Login creation object</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="403">Account email not verified</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiException), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiException), StatusCodes.Status429TooManyRequests)]
        public ActionResult<TokenDto> Login([FromBody] LoginDto loginDto)
        {
            var token = _accountService.Login(loginDto.Email, loginDto.Password, loginDto.Version);
            if (token == null)
            {
                Log.Information("Unsuccessful login for e-mail = {Email} from IP = {Ipaddress} ", loginDto.Email,
                    _httpContextAccessor.HttpContext.Connection.RemoteIpAddress);
                return Unauthorized();
            }

            return Ok(new TokenDto {Token = token});
        }

        /// <summary>
        /// Returns basic data about the account
        /// </summary>
        /// <returns>Account information</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<UserDto> Get()
        {
            var user = _accountService.GetAccountByClaims(User.Claims);
            var userDto = _mapperService.Map(user);
            var leaderBoardPlacement = _leaderboardService.GetLeaderboardPlacement(user);
            userDto.RankAllTime = leaderBoardPlacement.Total;
            userDto.RankSemester = leaderBoardPlacement.Semester;
            userDto.RankMonth = leaderBoardPlacement.Month;
            return Ok(userDto);
        }

        /// <summary>
        /// Updates the account and returns the updated values
        /// </summary>
        /// <param name="userDto">Update account information request</param>
        /// <returns>Account information</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<UserDto> Update([FromBody] UpdateUserDto userDto)
        {
            var user = _accountService.UpdateAccount(User.Claims, userDto);
            return Ok(_mapperService.Map(user));
        }

        /// <summary>
        /// </summary>
        /// <param name="emailDTO">Account email</param>
        /// <returns>Account information</returns>
        /// <response code="200">Successful request</response>
        /// <response code="400">Invalid request data model</response>
        [HttpPost("forgotpassword")]
        [AllowAnonymous]
        public IActionResult ForgotPassword([FromBody] EmailDto emailDTO)
        {
            _accountService.ForgotPassword(emailDTO.Email);

            Log.Information("Password reset requested for e-mail = {Email} from IP = {Ipaddress}", emailDTO.Email,
                _httpContextAccessor.HttpContext.Connection.RemoteIpAddress);

            return Ok("{ \"message\":\"We have send you a confirmation email\"}");
        }
    }
}