using System;
using System.Runtime.Serialization;

namespace CoffeeCard.Common.Errors
{
    [Serializable]
    public class UnauthorizedException : ApiException
    {
        public UnauthorizedException(string message)
            : base(message, 401) { }

        public UnauthorizedException(Exception ex)
            : base(ex.Message, 401) { }
    }
}
