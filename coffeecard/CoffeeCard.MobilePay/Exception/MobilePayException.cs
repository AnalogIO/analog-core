using System;
using System.Net;
using System.Runtime.Serialization;
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

        protected MobilePayException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            var errorMessage = info.GetValue("ErrorMessage", typeof(IMobilePayErrorMessage)) as IMobilePayErrorMessage;
            var statusCode =  info.GetValue("HttpStatusCode", typeof(HttpStatusCode));
            ArgumentNullException.ThrowIfNull(errorMessage);
            ArgumentNullException.ThrowIfNull(statusCode);
            _errorMessage = errorMessage;
            _httpStatusCode = (HttpStatusCode) statusCode;
        }

        public HttpStatusCode GetHttpStatusCode()
        {
            return _httpStatusCode;
        }

        public string GetErrorMessage()
        {
            return _errorMessage.GetErrorMessage();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("ErrorMessage", _errorMessage);
            info.AddValue("HttpStatusCode", _httpStatusCode);
        }

        public override string ToString()
        {
            return
                $"{base.ToString()}, {nameof(_errorMessage)}: {_errorMessage}, {nameof(_httpStatusCode)}: {_httpStatusCode}";
        }
    }
}