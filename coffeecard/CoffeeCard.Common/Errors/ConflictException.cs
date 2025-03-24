using Microsoft.AspNetCore.Http;

namespace CoffeeCard.Common.Errors
{
    public class ConflictException : ApiException
    {
        public ConflictException(string message) : base(message, statusCode: StatusCodes.Status409Conflict)
        {
        }
    }
}