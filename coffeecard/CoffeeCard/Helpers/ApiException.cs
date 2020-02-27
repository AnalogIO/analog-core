using System;

namespace CoffeeCard.WebApi.Helpers
{
    public class ApiException : Exception
    {
        public ApiException(string message, int statusCode = 500) : base(message)
        {
            StatusCode = statusCode;
        }

        public ApiException(Exception ex, int statusCode = 500) : base(ex.Message)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; set; }
    }
}