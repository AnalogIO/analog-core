namespace CoffeeCard.MobilePay.Exception.v2
{
    public class MobilePayApiException : System.Exception
    {
        public int StatusCode { get; }

        public string? ErrorCode { get; }

        public MobilePayApiException(int statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public MobilePayApiException(int statusCode, string message, string errorCode)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}
