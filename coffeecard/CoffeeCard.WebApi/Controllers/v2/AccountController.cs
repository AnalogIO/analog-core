using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers.v2
{
    /// <summary>
    /// Controller for creating and managing user accounts
    /// </summary>
    [ApiController]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Delete an account
        /// </summary>
        /// <param name="email">email of account to delete</param>
        /// <response code="204">Successful account deletion</response>
        /// <response code="404">Account not found</response>
        /// <response code="401">Invalid credentials</response>
        [HttpDelete("account")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiException), StatusCodes.Status429TooManyRequests)]
        public ActionResult Delete([FromBody] string email)
        {
            var exists = _accountService.UserExists(email);
            if (!exists) {
                return NotFound();
            }

            _accountService.AnonymizeAccount(email);
            return NoContent();
        }
    }
}