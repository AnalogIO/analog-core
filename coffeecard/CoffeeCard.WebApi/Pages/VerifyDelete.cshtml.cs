using System;
using CoffeeCard.Library.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public IActionResult OnGet()
        {
            try
            {
                _accountService.AnonymizeAccount(Token);
                
                TempData["resultHeader"] = "Success";
                TempData["result"] = @"Your account has been successfully deleted";
            }
            catch (Exception)
            {
                //FIXME: there was an error updating the user, how do we handle this?
                TempData["resultHeader"] = "Error";
                TempData["result"] =
                    @"Looks like the link you used has expired or already been used.";                
            }
            return RedirectToPage("result");
        }
    }
}