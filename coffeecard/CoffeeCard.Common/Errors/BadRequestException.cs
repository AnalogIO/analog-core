using System;
using Microsoft.AspNetCore.Http;

namespace CoffeeCard.Common.Errors;

public class BadRequestException : ApiException
{
    public BadRequestException(string message) : base(message, StatusCodes.Status400BadRequest)
    {
    }

    public BadRequestException(Exception ex) : base(ex, StatusCodes.Status400BadRequest)
    {
    }
}