using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using CoffeeCard.Helpers;
using CoffeeCard.Models.DataTransferObjects.User;
using CoffeeCard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace CoffeeCard.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IHostingEnvironment _env;
        private readonly ILeaderboardService _leaderboardService;
        private readonly IMapperService _mapperService;

        public AccountController(IAccountService accountService, ILeaderboardService leaderboardService,
            IMapperService mapperService, IHostingEnvironment env)
        {
            _accountService = accountService;
            _leaderboardService = leaderboardService;
            _mapperService = mapperService;
            _env = env;
        }

        /// <summary>
        ///     Registers the user supplied in the body and returns 201 on success
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ApiError), 400)]
        public ActionResult<CreatedResult> Register(RegisterDTO registerDto)
        {
            var user = _accountService.RegisterAccount(registerDto);
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
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        [ProducesResponseType(typeof(ApiError), 401)]
        public ActionResult<TokenDTO> Login(LoginDTO loginDto)
        {
            var token = _accountService.Login(loginDto.Email, loginDto.Password, loginDto.Version);
            if (token == null) return Unauthorized();
            return Ok(new TokenDTO {Token = token});
        }

        /// <summary>
        ///     Returns basic data about the user
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
        ///     Updates the user and returns the updated values
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
        ///     Returns the requested user id
        /// </summary>
        [HttpGet("lookupuserid")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        public ActionResult<UserDTO> LookUpUserId(int userId)
        {
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
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        public IActionResult ForgotPassword(EmailDTO emailDTO)
        {
            _accountService.ForgotPassword(emailDTO.Email);

            return Ok("{ \"message\":\"We have send you a confirmation email\"}");
        }

        /// <summary>
        ///     Verifies the user from a token delivered via email
        /// </summary>
        [AllowAnonymous]
        [HttpGet("verify")]
        public IActionResult Verify(string token)
        {
            var verified = _accountService.VerifyRegistration(token);
            var content = string.Empty;
            if (verified)
                content = "The account has been verified! You can now login into the app :D";
            else
                content = "The account could not be verified :-(";

            return new ContentResult
            {
                Content = content,
                ContentType = "text/html"
            };
        }

        [AllowAnonymous]
        [HttpGet("recover")]
        public HttpResponseMessage Recover(string token)
        {
            var response = new HttpResponseMessage();

            if (_accountService.RecoverUser(token))
            {
                var pathToTemplate = _env.WebRootPath
                                     + Path.DirectorySeparatorChar
                                     + "Templates"
                                     + Path.DirectorySeparatorChar
                                     + "EmailTemplate"
                                     + Path.DirectorySeparatorChar
                                     + "account_recover_success.html";

                var builder = new BodyBuilder();

                using (var SourceReader = System.IO.File.OpenText(pathToTemplate))
                {
                    builder.HtmlBody = SourceReader.ReadToEnd();
                }

                response.Content = new StringContent(builder.ToString());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return response;
            }

            response.Content = new StringContent("<html><body><h1>Token not found!</h1></body></html>");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}