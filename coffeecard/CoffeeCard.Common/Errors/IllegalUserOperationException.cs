using Microsoft.AspNetCore.Http;

namespace CoffeeCard.Common.Errors
{
    public class IllegalUserOperationException : ApiException
    {
        public IllegalUserOperationException(string message) : base(message, statusCode: StatusCodes.Status403Forbidden)
        {
        }
    }
}