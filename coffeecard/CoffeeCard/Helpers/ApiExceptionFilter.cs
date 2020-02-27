﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace CoffeeCard.WebApi.Helpers
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            ApiError apiError = null;
            if (context.Exception is ApiException)
            {
                // handle explicit 'known' API errors
                var ex = context.Exception as ApiException;
                context.Exception = null;
                apiError = new ApiError(ex.Message);

                context.HttpContext.Response.StatusCode = ex.StatusCode;
            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                apiError = new ApiError("Unauthorized Access");
                context.HttpContext.Response.StatusCode = 401;

                // handle logging here
            }
            else
            {
                // Unhandled errors
#if !DEBUG
                                var msg = "An unhandled error occurred.";
#else
                var msg = context.Exception.GetBaseException().Message;
#endif

                apiError = new ApiError(msg);

                context.HttpContext.Response.StatusCode = 500;

                Log.Error("Unhandled exception caught. Exception={exception}", context.Exception);
            }

            // always return a JSON result
            context.Result = new JsonResult(apiError);

            base.OnException(context);
        }
    }
}