using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Linq;

namespace CoffeeCard.WebApi.Helpers.Swagger
{
    /// <summary>
    /// Custom processor for OpenApi documents. Checks for parameters with name 'version' and removes them from the list of parameters.
    /// This is to remove the API Version path parameter field from the OpenApi specification
    /// Inspired by <see cref="ApiVersionProcessor"/>
    /// </summary>
    public class RemoveApiVersionProcessor : IOperationProcessor
    {
        /// <summary>
        /// Process OpenApi document and remove parameters with Parameter Name 'version'
        /// </summary>
        /// <returns>true if any matching parameters found, false if not</returns>
        public bool Process(OperationProcessorContext context)
        {
            NSwag.OpenApiOperationDescription operationDescription = context.OperationDescription;
            System.Collections.Generic.List<NSwag.OpenApiParameter> versionParameter = operationDescription.Operation.Parameters.Where(p => p.Name == "version").ToList();

            if (versionParameter.Any())
            {
                foreach (NSwag.OpenApiParameter parameter in versionParameter)
                {
                    _ = context.OperationDescription.Operation.Parameters.Remove(parameter);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}