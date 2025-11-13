using System;
using System.Threading.Tasks;
using CoffeeCard.Library.Services;
using CoffeeCard.WebApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoffeeCard.WebApi.Pages
{
    /// <summary>
    /// Represents the page model for verifying a user's email address.
    /// </summary>
    public class VerifyEmail : PageModel
    {
        private readonly IAccountService _accountService;

        /// <summary>
        /// Initializes a new instance of the <see cref="VerifyEmail"/> class.
        /// </summary>
        /// <param name="accountService">The account service.</param>
        public VerifyEmail(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Gets or sets the token used for email verification.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        /// <summary>
        /// Handles GET requests for email verification.
        /// </summary>
        public async Task<IActionResult> OnGet()
        {
            return await PageUtils.SafeExecuteFunc(Func, this);

            async Task<IActionResult> Func()
            {
                var emailVerified = await _accountService.VerifyRegistration(Token);
                return RedirectToPage(
                    "Result",
                    emailVerified
                        ? new { action = "verifyEmail", outcome = "success" }
                        : new { action = "verifyEmail", outcome = "error" }
                );
            }
        }
    }
}
