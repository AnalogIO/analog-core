using System;
using System.Threading.Tasks;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.PagesModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        [BindProperty] public NewPinCodeModel PinCode { get; set; }

        [BindProperty(SupportsGet = true)] public string Token { get; set; }

        private bool IsTokenValid { get; set; }

        public async Task<IActionResult> OnGet()
        {
            try
            {
                IsTokenValid = await _tokenService.ValidateTokenIsUnusedAsync(Token);

                if (IsTokenValid) return Page();
            }
            catch (Exception e)
            {
                // ignored
            }

            TempData["resultHeader"] = "Error";
            TempData["result"] =
                @"Looks like the link you used has expired or already been used. Request a new one from the app.";
            return RedirectToPage("result");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            try
            {
                if (await _accountService.RecoverUserAsync(Token, PinCode.NewPinCode))
                {
                    TempData["resultHeader"] = "Success";
                    TempData["result"] = "Your password has now been reset";
                }
            }
            catch (Exception e)
            {
                TempData["resultHeader"] = "Something went wrong";
                TempData["result"] =
                    "An error occured while updating your pin code. Please try again later or contact us at support@analogio.dk for further support";
            }

            return RedirectToPage("result");
        }
    }
}