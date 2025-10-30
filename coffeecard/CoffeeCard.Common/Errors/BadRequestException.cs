using System;

namespace CoffeeCard.Common.Errors;

public class BadRequestException : ApiException
{
    public BadRequestException(string message, int statusCode = 400)
        : base(message, statusCode) { }

    public BadRequestException(Exception ex, int statusCode = 400)
        : base(ex, statusCode) { }
}
