using System;
using System.Runtime.Serialization;

namespace CoffeeCard.WebApi.Helpers
{
    [Serializable]
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

        protected ApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            StatusCode = info.GetInt32("StatusCode");
        }

        public int StatusCode { get; private set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("StatusCode", StatusCode);
        }
    }
}