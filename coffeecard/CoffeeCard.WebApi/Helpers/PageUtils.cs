using System;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.WebApi.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace CoffeeCard.WebApi.Helpers
{
    /// <summary>
    /// Convenience methods to be used on the user-facing webpages
    /// </summary>
    public static class PageUtils
    {
        /// <summary>
        /// Executes a function safely and handles any exceptions that may occur.
        /// </summary>
        /// <param name="func">The function to execute.</param>
        /// <param name="model">The page model to use for setting error messages.</param>
        /// <returns>An IActionResult representing the result of the function execution.</returns>
        public static async Task<IActionResult> SafeExecuteFunc(
            Func<Task<IActionResult>> func,
            PageModel model
        )
        {
            try
            {
                return await Task.Run(func);
            }
            catch (Exception ex)
            {
                var error =
                    ex is UnauthorizedException
                        ? Outcome.LinkExpiredOrUsed
                        : Outcome.UnhandledError;

                if (ex is not ApiException)
                {
                    Log.Error(ex, "An unhandled exception occured on a webpage");
                }

                return model.RedirectToPage("result", new { outcome = error });
            }
        }
    }
}
