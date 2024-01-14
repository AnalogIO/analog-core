using System;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;

namespace CoffeeCard.Common.Errors
{
    [Serializable]
    public class EntityNotFoundException : ApiException
    {
        public EntityNotFoundException(string message)
            : base(message, statusCode: StatusCodes.Status404NotFound) { }

        protected EntityNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
