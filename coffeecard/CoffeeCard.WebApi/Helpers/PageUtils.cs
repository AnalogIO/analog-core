using System;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using Microsoft.AspNetCore.Http;
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
        public static async Task<IActionResult> SafeExecuteFunc(
            Func<Task<IActionResult>> func,
            PageModel model
        )
        {
            try
            {
                return await Task.Run(func);
            }
            catch (ApiException ex)
            {
                // Not logging this, since we should have logged the cause when we created it.
                if (ex.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    setMessage(
                        "Error",
                        "Looks like the link you used has expired or already been used. Request a new link and try again",
                        model
                    );
                }
                else
                {
                    setMessage("Error", ex.Message, model);
                }
            }
            catch (Exception ex)
            {
                setMessage("Error", "An unhandled error occured. Try again later", model);
                Log.Error(ex, "An unhandled exception occured on a webpage");
            }
            return model.RedirectToPage("result");
        }

        public static void setMessage(string header, string message, PageModel model)
        {
            model.TempData["resultHeader"] = header;
            model.TempData["result"] = message;
        }
    }
}
