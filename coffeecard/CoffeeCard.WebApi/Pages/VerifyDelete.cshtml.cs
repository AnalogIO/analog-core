using CoffeeCard.Library.Services.v2;
using CoffeeCard.WebApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace CoffeeCard.WebApi.Pages
{
    public class VerifyDelete : PageModel
    {
        private readonly IAccountService _accountService;

        public VerifyDelete(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty(SupportsGet = true)] public string Token { get; set; }

        public async Task<IActionResult> OnGet()
        {
            Func<Task<IActionResult>> func = async delegate ()
            {
                await _accountService.AnonymizeAccountAsync(Token);

                TempData["resultHeader"] = "Success";
                TempData["result"] = @"Your account has been successfully deleted";

                return RedirectToPage("result");
            };
            return await PageUtils.SafeExecuteFunc(func, this);
        }
    }
}