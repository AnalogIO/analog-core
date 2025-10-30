using System;

namespace CoffeeCard.Common.Errors
{
    [Serializable]
    public class ApiException : Exception
    {
        public int StatusCode { get; }

        public ApiException(string message, int statusCode = 500)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public ApiException(Exception ex, int statusCode = 500)
            : base(ex.Message)
        {
            StatusCode = statusCode;
        }
    }
}
