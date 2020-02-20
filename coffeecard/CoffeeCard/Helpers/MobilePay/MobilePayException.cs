using System;
using System.Net;
using CoffeeCard.Helpers.MobilePay.ErrorMessage;

namespace CoffeeCard.Helpers.MobilePay
{
    [Serializable]
    public class MobilePayException : Exception
    {
        private readonly IMobilePayErrorMessage _errorMessage;
        private readonly HttpStatusCode _httpStatusCode;

        public MobilePayException(IMobilePayErrorMessage errorMessage, HttpStatusCode statusCode)
            : base(errorMessage.GetErrorMessage())
        {
            _errorMessage = errorMessage;
            _httpStatusCode = statusCode;
        }

        public HttpStatusCode GetHttpStatusCode()
        {
            return _httpStatusCode;
        }

        public string GetErrorMessage()
        {
            return _errorMessage.GetErrorMessage();
        }
    }
}