using System;
using Microsoft.AspNetCore.Http;

namespace CoffeeCard.Common.Errors
{
    [Serializable]
    public class EntityNotFoundException : ApiException
    {
        public EntityNotFoundException(string message)
            : base(message, statusCode: StatusCodes.Status404NotFound) { }
    }
}
