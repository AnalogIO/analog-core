using System;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;

namespace CoffeeCard.Common.Errors
{
    [Serializable]
    public class ConflictExceptiion : ApiException
    {
        public ConflictExceptiion(string message) : base(message, statusCode: StatusCodes.Status409Conflict)
        {
        }

        protected ConflictExceptiion(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}