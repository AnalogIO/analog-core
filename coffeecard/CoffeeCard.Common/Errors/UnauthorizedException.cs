using System;
using Microsoft.AspNetCore.Http;

namespace CoffeeCard.Common.Errors
{
    public class UnauthorizedException : ApiException
    {
        public UnauthorizedException(string message) : base(message, StatusCodes.Status401Unauthorized)
        { }

        public UnauthorizedException(Exception ex) : base(ex.Message, StatusCodes.Status401Unauthorized)
        { }
    }
}