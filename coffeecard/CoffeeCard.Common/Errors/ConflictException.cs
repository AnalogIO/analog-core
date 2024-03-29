using System;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;

namespace CoffeeCard.Common.Errors
{
    [Serializable]
    public class ConflictException : ApiException
    {
        public ConflictException(string message) : base(message, statusCode: StatusCodes.Status409Conflict)
        {
        }

        protected ConflictException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}