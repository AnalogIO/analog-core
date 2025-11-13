using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.WebApi.Helpers;

public class RequestLoggerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggerMiddleware> _logger;

    public RequestLoggerMiddleware(RequestDelegate next, ILogger<RequestLoggerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log the Request
        var request = await FormatRequest(context.Request);
        _logger.LogDebug("Incoming Request: {Request}", request);

        // Copy original response body stream
        var originalResponseBodyStream = context.Response.Body;

        using (var responseBody = new MemoryStream())
        {
            context.Response.Body = responseBody;

            // Call the next middleware in the pipeline
            await _next(context);

            // Log the Response
            var response = await FormatResponse(context.Response);
            _logger.LogDebug("Outgoing Response: {Response}", response);

            // Copy the response body back to the original stream
            await responseBody.CopyToAsync(originalResponseBodyStream);
        }
    }

    private async Task<string> FormatRequest(HttpRequest request)
    {
        request.EnableBuffering();

        var bodyAsText = string.Empty;
        if (request.Body.CanSeek)
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true))
            {
                bodyAsText = await reader.ReadToEndAsync();
            }

            request.Body.Seek(0, SeekOrigin.Begin);
        }

        var headers = FormatHeaders(request.Headers);

        return $"Method: {request.Method}, Path: {request.Path}, QueryString: {request.QueryString}, Headers: {headers}, Body: {bodyAsText}";
    }

    private async Task<string> FormatResponse(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        return $"StatusCode: {response.StatusCode}, Body: {text}";
    }

    private string FormatHeaders(IHeaderDictionary headers)
    {
        var formattedHeaders = new StringBuilder();

        foreach (
            KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> header in headers
        )
        {
            formattedHeaders.AppendLine($"{header.Key}: {header.Value}");
        }

        return formattedHeaders.ToString().TrimEnd();
    }
}
