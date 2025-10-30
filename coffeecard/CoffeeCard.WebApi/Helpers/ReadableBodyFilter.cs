using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

// For ASP.NET 3.1

namespace CoffeeCard.WebApi.Helpers
{
    /// <summary>
    /// A filter that enables the request body to be read multiple times.
    /// </summary>
    public class ReadableBodyFilter : IResourceFilter
    {
        /// <inheritdoc/>
        public void OnResourceExecuted(ResourceExecutedContext context) { }

        /// <summary>
        /// Enables the request body to be read multiple times.
        /// </summary>
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            context.HttpContext.Request.EnableBuffering();
        }
    }
}
