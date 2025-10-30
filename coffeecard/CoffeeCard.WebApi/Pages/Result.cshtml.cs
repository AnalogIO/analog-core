using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoffeeCard.WebApi.Pages
{
    /// <summary>
    /// Represents the result page model for the CoffeeCard web application.
    /// </summary>
    public class Result : PageModel
    {
        /// <summary>
        /// Handles GET requests for the result page.
        /// </summary>
        /// <returns>The result page.</returns>
        public IActionResult OnGet()
        {
            return Page();
        }
    }
}
