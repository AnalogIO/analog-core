using System;
using CoffeeCard.Common.Errors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.WebApi.Helpers
{
    /// <summary>
    /// Exception filter for handling API exceptions and returning appropriate HTTP status codes and error messages.
    /// </summary>
    public class ApiExceptionFilter(IWebHostEnvironment environment, ILogger<ApiExceptionFilter> logger)
        : ExceptionFilterAttribute
    {
        private readonly ILogger<ApiExceptionFilter> _logger = logger;
        private readonly IWebHostEnvironment _environment = environment;

        /// <inheritdoc/>
        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            var responseMessage = GetResponseMessage(exception);
            var apiError = new ApiError(responseMessage);
            context.HttpContext.Response.StatusCode = GetStatusCode(exception);
            LogException(exception);

            // always return a JSON result
            context.Result = new JsonResult(apiError);
            context.ExceptionHandled = true;

            base.OnException(context);
        }

        private string GetResponseMessage(Exception exception)
        {
            return exception switch
            {
                // We consider our own exceptions fine for sending to users
                ApiException apiException => apiException.Message,
                // To avoid leaking internal errors, we only show the exception message in dev
                _ => _environment.IsDevelopment()
                    ? exception.GetBaseException().Message
                    : "Unhandled exception caught"
            };
        }

        private int GetStatusCode(Exception exception)
        {
            return exception switch
            {
                ApiException apiException => apiException.StatusCode,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        private void LogException(Exception exception)
        {
            if (exception is ApiException apiException)
            {
                _logger.LogWarning(apiException, "Unintended user flow occured");
            }
            else
            {
                _logger.LogError(exception, "Unhandled exception caught");
            }
        }
    }
}