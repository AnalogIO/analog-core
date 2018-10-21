using coffeecard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace coffeecard.Controllers
{
    [ApiVersion("1")]
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AccountController : ControllerBase
    {
        IAccountService _accountService;
        IMapperService _mapperService;

        public AccountController(IAccountService accountService, IMapperService mapperService)
        {
            _accountService = accountService;
            _mapperService = mapperService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public ActionResult Register(RegisterDTO registerDto)
        {
            var user = _accountService.RegisterAccount(registerDto);
            return Created("register", new { message = "Your user has been created! Please check your email to verify your account.\n(Check your spam folder!)" });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public ActionResult Login(LoginDTO loginDto)
        {
            var token = _accountService.Login(loginDto.Email, loginDto.Password, loginDto.Version);
            if(token == null) return Unauthorized();
            return Ok(new { token = token });
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public ActionResult Get()
        {
            var user = _accountService.GetAccountByClaims(User.Claims);
            return Ok(_mapperService.Map(user));
        }

        [AllowAnonymous]
        [HttpGet("verify")]
        public IActionResult Verify(string token)
        {
            var verified = _accountService.VerifyRegistration(token);
            var content = string.Empty;
            if(verified)
            {
                content = $"The account has been verified! You can now login into the app :D";
            } else
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