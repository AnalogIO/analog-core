using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CoffeeCard.WebApi.Helpers.Swagger;

/// <summary>
/// Marks request bodies as required, matching the controller contract.
/// </summary>
public sealed class RequiredRequestBodyFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.RequestBody is OpenApiRequestBody requestBody)
            requestBody.Required = true;
    }
}
