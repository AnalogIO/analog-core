using System;
using System.Net;
using CoffeeCard.MobilePay.ErrorMessage;

namespace CoffeeCard.MobilePay.Exception
{
    [Serializable]
    public class MobilePayException : System.Exception
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

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(_errorMessage)}: {_errorMessage}, {nameof(_httpStatusCode)}: {_httpStatusCode}";
        }
    }
}