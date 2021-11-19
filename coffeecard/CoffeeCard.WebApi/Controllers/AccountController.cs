using CoffeeCard.WebApi.Models.DataTransferObjects.User;
using CoffeeCard.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CoffeeCard.WebApi.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILeaderboardService _leaderboardService;
        private readonly IMapperService _mapperService;

        public AccountController(IAccountService accountService, ILeaderboardService leaderboardService,
            IMapperService mapperService, IHttpContextAccessor httpContextAccessor)
        {
            _accountService = accountService;
            _leaderboardService = leaderboardService;
            _mapperService = mapperService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Registers the user supplied in the body and returns 201 on success
        /// </summary>
        /// <param name="registerDto">Register data object</param>
        /// <response code="201">Successful user creation</response>
        /// <response code="400">Invalid User creation object</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Register(RegisterDto registerDto)
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
        /// Returns a token that is used to identify the user
        /// </summary>
        /// <returns>Login token</returns>
        /// <param name="loginDto">Login data object</param>
        /// <response code="200">Successful user login</response>
        /// <response code="400">Invalid Login creation object</response>
        /// <response code="401">Invalid Login credentials</response>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<TokenDto> Login(LoginDto loginDto)
        {
            var token = _accountService.Login(loginDto.Email, loginDto.Password, loginDto.Version);
            if (token == null)
            {
                Log.Information("Unsuccessful login for e-mail = {email} from IP = {ipadress} ", loginDto.Email,
                    _httpContextAccessor.HttpContext.Connection.RemoteIpAddress);
                return Unauthorized();
            }

            return Ok(new TokenDto {Token = token});
        }

        /// <summary>
        /// Returns basic data about the user
        /// </summary>
        /// <returns>User information</returns>
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
        /// Updates the user and returns the updated values
        /// </summary>
        /// <param name="userDto">Update user information request</param>
        /// <returns>User information</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<UserDto> Update(UpdateUserDto userDto)
        {
            var user = _accountService.UpdateAccount(User.Claims, userDto);
            return Ok(_mapperService.Map(user));
        }

        /// <summary>
        /// Sends email to user if they forgot password
        /// </summary>
        /// <param name="emailDTO">User email</param>
        /// <returns>User information</returns>
        /// <response code="200">Successful request</response>
        /// <response code="400">Invalid request data model</response>
        [AllowAnonymous]
        [HttpPost("forgotpassword")]
        public IActionResult ForgotPassword(EmailDto emailDTO)
        {
            _accountService.ForgotPassword(emailDTO.Email);

            Log.Information("Password reset requested for e-mail = {email} from IP = {ipaddress}", emailDTO.Email,
                _httpContextAccessor.HttpContext.Connection.RemoteIpAddress);

            return Ok("{ \"message\":\"We have send you a confirmation email\"}");
        }
    }
}