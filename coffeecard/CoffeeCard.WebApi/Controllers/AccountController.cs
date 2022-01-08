using CoffeeCard.WebApi.Helpers;
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
        ///     Registers the user supplied in the body and returns 201 on success
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
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
        ///     Returns a token that is used to identify the user
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginDto loginDto)
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
        ///     Returns basic data about the user
        /// </summary>
        [HttpGet]
        public IActionResult Get()
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
        ///     Updates the user and returns the updated values
        /// </summary>
        [HttpPut]
        public IActionResult Update(UpdateUserDto userDto)
        {
            var user = _accountService.UpdateAccount(User.Claims, userDto);
            return Ok(_mapperService.Map(user));
        }

        /// <summary>
        ///     Returns the requested user id
        /// </summary>
        [HttpGet("lookupuserid")]
        public IActionResult LookUpUserId(int userId)
        {
            var authorizedUser = _accountService.GetAccountByClaims(User.Claims);
            if (authorizedUser.Id != userId)
            {
                throw new ApiException("Users may only request information for their own account", 403);
            }
            var user = _accountService.GetUserById(userId);
            return Ok(_mapperService.Map(user));
        }

        /// <summary>
        ///     Sends email to user if they forgot password
        /// </summary>
        /// <param name="emailDTO"></param>
        /// <returns></returns>
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