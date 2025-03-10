using System;
using System.Threading.Tasks;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.PagesModels;
using CoffeeCard.WebApi.Helpers;
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
        [BindProperty] public NewPinCodeModel PinCode { get; set; }

        /// <summary>
        /// Represents the token used for password recovery.
        /// </summary>
        [BindProperty(SupportsGet = true)] public string Token { get; set; }

        private bool IsTokenValid { get; set; }

        /// <summary>
        /// Handles the HTTP GET request for the password recovery page.
        /// Validates the token and displays the password recovery page if the token is valid.
        /// Otherwise, redirects to the result page with an error message.
        /// </summary>
        /// <returns>The action result of the password recovery page or the result page.</returns>
        public async Task<IActionResult> OnGet()
        {
            Func<Task<IActionResult>> func = async delegate ()
            {
                IsTokenValid = await _tokenService.ValidateTokenIsUnusedAsync(Token);

                if (IsTokenValid) return Page();
                else
                {
                    PageUtils.setMessage("Error",
                        "Looks like the link you used has expired or already been used. Request a new password in the app to verify your email.",
                        this);
                    return RedirectToPage("result");
                }
            };

            return await PageUtils.SafeExecuteFunc(func, this);
        }

        /// <summary>
        /// Handles the HTTP POST request for the password recovery page.
        /// </summary>
        /// <returns>The action result of the password recovery page.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();


            Func<Task<IActionResult>> func = async delegate ()
            {
                if (await _accountService.RecoverUserAsync(Token, PinCode.NewPinCode))
                {
                    PageUtils.setMessage("Success", "Your password has now been reset", this);
                }
                else
                {
                    PageUtils.setMessage("Error", "An error occured while updating your pin code. Please try again later or contact us at support@analogio.dk for further support", this);
                }
                return RedirectToPage("result");
            };

            return await PageUtils.SafeExecuteFunc(func, this);
        }
    }
}