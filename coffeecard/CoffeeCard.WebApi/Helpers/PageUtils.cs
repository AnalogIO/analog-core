using System;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.WebApi.Pages.Shared;
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
        /// <returns>An Outcome representing the result of the function execution.</returns>
        public static async Task<Outcome> SafeExecute(Func<Task<Outcome>> func)
        {
            try
            {
                return await func();
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

                return error;
            }
        }
    }
}
