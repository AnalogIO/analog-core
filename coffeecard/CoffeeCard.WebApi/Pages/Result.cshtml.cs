using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoffeeCard.WebApi.Pages
{
    public class Result : PageModel
    {
        public IActionResult OnGet()
        {
            return Page();
        }
    }
}