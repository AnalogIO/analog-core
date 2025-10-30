using System;
using CoffeeCard.Common.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace CoffeeCard.WebApi.Helpers
{
    /// <summary>
    /// Exception filter for handling API exceptions and returning appropriate HTTP status codes and error messages.
    /// </summary>
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        /// <inheritdoc/>
        public override void OnException(ExceptionContext context)
        {
            ApiError apiError;
            switch (context.Exception)
            {
                case ConflictException exception:
                    apiError = new ApiError(exception.Message);
                    context.HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                    break;
                case ApiException exception:
                {
                    apiError = new ApiError(exception.Message);
                    context.HttpContext.Response.StatusCode = exception.StatusCode;
                    break;
                }
                case UnauthorizedAccessException _:
                    apiError = new ApiError("Unauthorized Access");
                    context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    break;
                default:
                {
                    Log.Error(context.Exception, "Unhandled exception caught");

#if !DEBUG
                    var msg = "An unhandled error occurred.";
#else
                    var msg = context.Exception.GetBaseException().Message;
#endif

                    apiError = new ApiError(msg);
                    context.HttpContext.Response.StatusCode =
                        StatusCodes.Status500InternalServerError;
                    break;
                }
            }

            // always return a JSON result
            context.Result = new JsonResult(apiError);

            base.OnException(context);
        }
    }
}
