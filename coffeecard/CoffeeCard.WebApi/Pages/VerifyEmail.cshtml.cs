﻿using System;
using System.Threading.Tasks;
using CoffeeCard.Library.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoffeeCard.WebApi.Pages
{
    public class VerifyEmail : PageModel
    {
        private readonly IAccountService _accountService;

        public VerifyEmail(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty(SupportsGet = true)] public string Token { get; set; }

        public async Task<IActionResult> OnGet()
        {
            try
            {
                var emailVerified = await _accountService.VerifyRegistration(Token);
                if (emailVerified)
                {
                    TempData["resultHeader"] = "Success";
                    TempData["result"] = @"Your email has been successfully verified";
                    return RedirectToPage("result");
                }
            }
            catch (Exception)
            {
                TempData["resultHeader"] = "Error";
                TempData["result"] =
                    @"Looks like the link you used has expired or already been used. Request a new password in the app to verify your email.";
            }
            
            return RedirectToPage("result");
        }
    }
}