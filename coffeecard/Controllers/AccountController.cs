using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace coffeecard.Controllers
{
    [ApiVersion("1")]
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AccountController : ControllerBase
    {
        IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public ActionResult Register(RegisterDTO registerDto)
        {
            var user = _accountService.RegisterAccount(registerDto);
            return Created("register", new { message = "Your user has been created! Please check your email to verify your account.\n(Check your spam folder!)" });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult Login(LoginDTO loginDto)
        {
            var token = _accountService.Login(loginDto.Email, loginDto.Password, loginDto.Version);
            if(token == null) return Unauthorized();
            return Ok(new { token = token });
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