using System;
using System.Threading.Tasks;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.PagesModels;
using CoffeeCard.WebApi.Helpers;
using CoffeeCard.WebApi.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoffeeCard.WebApi.Pages
{
    /// <summary>
    /// Represents the page model for the password recovery page.
    /// </summary>
    public class Recover : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="Recover"/> class.
        /// </summary>
        /// <param name="accountService">The account service.</param>
        /// <param name="tokenService">The token service.</param>
        public Recover(IAccountService accountService, ITokenService tokenService)
        {
            _accountService = accountService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Represents the model for the new pin code.
        /// </summary>
        [BindProperty]
        public NewPinCodeModel PinCode { get; set; }

        /// <summary>
        /// Represents the token used for password recovery.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        private bool IsTokenValid { get; set; }

        /// <summary>
        /// Handles the HTTP GET request for the password recovery page.
        /// Validates the token and displays the password recovery page if the token is valid.
        /// Otherwise, redirects to the result page with an error message.
        /// </summary>
        /// <returns>The action result of the password recovery page or the result page.</returns>
        public async Task<IActionResult> OnGet()
        {
            return await PageUtils.SafeExecuteFunc(Func, this);

            async Task<IActionResult> Func()
            {
                IsTokenValid = await _tokenService.ValidateTokenIsUnusedAsync(Token);

                if (IsTokenValid)
                    return Page();
                else
                {
                    return RedirectToPage("result", new { outcome = Outcome.LinkExpiredOrUsed });
                }
            }
        }

        /// <summary>
        /// Handles the HTTP POST request for the password recovery page.
        /// </summary>
        /// <returns>The action result of the password recovery page.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            return await PageUtils.SafeExecuteFunc(Func, this);

            async Task<IActionResult> Func()
            {
                if (await _accountService.RecoverUserAsync(Token, PinCode.NewPinCode))
                {
                    return RedirectToPage("result", new { outcome = Outcome.PasswordResetSuccess });
                }
                else
                {
                    return RedirectToPage("result", new { outcome = Outcome.PinUpdateError });
                }
            }
        }
    }
}
