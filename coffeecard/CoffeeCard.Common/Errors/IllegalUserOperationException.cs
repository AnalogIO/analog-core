using Microsoft.AspNetCore.Http;
using System;
using System.Runtime.Serialization;

namespace CoffeeCard.Common.Errors
{
    [Serializable]
    public class IllegalUserOperationException : ApiException
    {
        public IllegalUserOperationException(string message) : base(message, statusCode: StatusCodes.Status403Forbidden)
        {
        }

        protected IllegalUserOperationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}