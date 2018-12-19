using coffeecard.Helpers;
using coffeecard.Models.DataTransferObjects.User;
using coffeecard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace coffeecard.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiController]
    public class AccountController : ControllerBase
    {
        IAccountService _accountService;
        ILeaderboardService _leaderboardService;
        IMapperService _mapperService;

        public AccountController(IAccountService accountService, ILeaderboardService leaderboardService, IMapperService mapperService)
        {
            _accountService = accountService;
            _leaderboardService = leaderboardService;
            _mapperService = mapperService;
        }

        /// <summary>
        ///  Registers the user supplied in the body and returns 201 on success
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ApiError), 400)]
        public ActionResult<CreatedResult> Register(RegisterDTO registerDto)
        {
            var user = _accountService.RegisterAccount(registerDto);
            return Created("register", new { message = "Your user has been created! Please check your email to verify your account.\n(Check your spam folder!)" });
        }

        /// <summary>
        ///  Returns a token that is used to identify the user
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        [ProducesResponseType(typeof(ApiError), 401)]
        public ActionResult<TokenDTO> Login(LoginDTO loginDto)
        {
            var token = _accountService.Login(loginDto.Email, loginDto.Password, loginDto.Version);
            if (token == null) return Unauthorized();
            return Ok(new TokenDTO { Token = token });
        }

        /// <summary>
        ///  Returns basic data about the user
        /// </summary>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public ActionResult<UserDTO> Get()
        {
            var user = _accountService.GetAccountByClaims(User.Claims);
            var userDTO = _mapperService.Map(user);
            var leaderBoardPlacement = _leaderboardService.GetLeaderboardPlacement(user);
            userDTO.RankAllTime = leaderBoardPlacement.Total;
            userDTO.RankSemester = leaderBoardPlacement.Semester;
            userDTO.RankMonth = leaderBoardPlacement.Month;
            return Ok(userDTO);
        }

        /// <summary>
        ///  Updates the user and returns the updated values
        /// </summary>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        [ProducesResponseType(typeof(ApiError), 401)]
        public ActionResult<UserDTO> Update(UpdateUserDTO userDto)
        {
            var user = _accountService.UpdateAccount(User.Claims, userDto);
            return Ok(_mapperService.Map(user));
        }

        /// <summary>
        ///  Verifies the user from a token delivered via email
        /// </summary>
        [AllowAnonymous]
        [HttpGet("verify")]
        public IActionResult Verify(string token)
        {
            var verified = _accountService.VerifyRegistration(token);
            var content = string.Empty;
            if (verified)
            {
                content = $"The account has been verified! You can now login into the app :D";
            }
            else
            {
                content = $"The account could not be verified :-(";
            }

            return new ContentResult()
            {
                Content = content,
                ContentType = "text/html",
            };
        }
    }
}