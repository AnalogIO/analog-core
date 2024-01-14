using System;
using System.Threading.Tasks;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.PagesModels;
using CoffeeCard.WebApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace CoffeeCard.WebApi.Pages
{
    public class Recover : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;

        public Recover(IAccountService accountService, ITokenService tokenService)
        {
            _accountService = accountService;
            _tokenService = tokenService;
        }

        [BindProperty]
        public NewPinCodeModel PinCode { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        private bool IsTokenValid { get; set; }

        public async Task<IActionResult> OnGet()
        {
            Func<Task<IActionResult>> func = async delegate()
            {
                IsTokenValid = await _tokenService.ValidateTokenIsUnusedAsync(Token);

                if (IsTokenValid)
                    return Page();
                else
                {
                    PageUtils.setMessage(
                        "Error",
                        "Looks like the link you used has expired or already been used. Request a new password in the app to verify your email.",
                        this
                    );
                    return RedirectToPage("result");
                }
            };

            return await PageUtils.SafeExecuteFunc(func, this);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            Func<Task<IActionResult>> func = async delegate()
            {
                if (await _accountService.RecoverUserAsync(Token, PinCode.NewPinCode))
                {
                    PageUtils.setMessage("Success", "Your password has now been reset", this);
                }
                else
                {
                    PageUtils.setMessage(
                        "Error",
                        "An error occured while updating your pin code. Please try again later or contact us at support@analogio.dk for further support",
                        this
                    );
                }
                return RedirectToPage("result");
            };

            return await PageUtils.SafeExecuteFunc(func, this);
        }
    }
}
