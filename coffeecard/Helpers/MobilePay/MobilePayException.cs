using System;
using System.Net;
using System.Runtime.Serialization;
using coffeecard.Helpers.MobilePay.ErrorMessage;

namespace coffeecard.Helpers.MobilePay
{
    [Serializable]
    internal class MobilePayException : Exception
    {
        private readonly IMobilePayErrorMessage _errorMessage;
        private readonly HttpStatusCode _httpStatusCode;

        public MobilePayException(IMobilePayErrorMessage errorMessage, HttpStatusCode statusCode) 
            : base(errorMessage.GetErrorMessage())
        {
            _errorMessage = errorMessage;
            _httpStatusCode = statusCode;
        }

        public HttpStatusCode GetHttpStatusCode() { return _httpStatusCode; }

        public string GetErrorMessage() { return _errorMessage.GetErrorMessage(); }
    }
}