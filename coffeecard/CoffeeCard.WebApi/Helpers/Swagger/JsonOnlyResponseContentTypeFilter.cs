using System.Collections.Generic;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CoffeeCard.WebApi.Helpers.Swagger;

/// <summary>
/// Ensures generated OpenAPI operations only advertise a single, canonical
/// JSON media type for both request and response payloads.
///
/// Swashbuckle (via ApiExplorer) can include several JSON-like media types
/// such as "text/json" or "application/*+json" when an action uses JSON
/// serializers or returns strings. That causes clients and generated SDKs
/// to think multiple content-types are acceptable. This filter normalizes
/// `requestBody.content` and `response.content` to only contain
/// "application/json", which matches our API contract and simplifies
/// client generation and documentation.
/// </summary>
public sealed class JsonOnlyResponseContentTypeFilter : IOperationFilter
{
    private const string JsonMediaType = "application/json";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Swagger should advertise the same JSON content type for both payload directions.
        if (operation.RequestBody is not null)
        {
            NormalizeContentTypes(operation.RequestBody.Content);
        }

        foreach (var response in operation.Responses.Values)
        {
            NormalizeContentTypes(response.Content);
        }
    }

    private static void NormalizeContentTypes(IDictionary<string, OpenApiMediaType> content)
    {
        if (content.Count == 0 || !content.ContainsKey(JsonMediaType))
        {
            return;
        }

        var jsonContent = content[JsonMediaType];
        content.Clear();
        content[JsonMediaType] = jsonContent;
    }
}
