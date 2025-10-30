using System.Linq;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

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
            var operationDescription = context.OperationDescription;
            var versionParameter = operationDescription
                .Operation.Parameters.Where(p => p.Name == "version")
                .ToList();

            if (versionParameter.Any())
            {
                foreach (var parameter in versionParameter)
                {
                    context.OperationDescription.Operation.Parameters.Remove(parameter);
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
