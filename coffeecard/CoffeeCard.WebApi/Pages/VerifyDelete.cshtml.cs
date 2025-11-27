using System;
using System.Threading.Tasks;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.WebApi.Helpers;
using CoffeeCard.WebApi.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoffeeCard.WebApi.Pages
{
    /// <summary>
    /// Page model for verifying and deleting a user's account.
    /// </summary>
    public class VerifyDelete : PageModel
    {
        private readonly IAccountService _accountService;

        /// <summary>
        /// Initializes a new instance of the <see cref="VerifyDelete"/> class.
        /// </summary>
        /// <param name="accountService">The account service.</param>
        public VerifyDelete(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Token used to verify and delete a user's account.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        /// <summary>
        /// Handles the GET request for verifying and deleting a user's account.
        /// </summary>
        /// <returns>The result of the action.</returns>
        public async Task<IActionResult> OnGet()
        {
            return await PageUtils.SafeExecuteFunc(Func, this);

            async Task<IActionResult> Func()
            {
                await _accountService.AnonymizeAccountAsync(Token);
                return RedirectToPage("Result", new { outcome = Outcome.AccountDeletedSuccess });
            }
        }
    }
}
