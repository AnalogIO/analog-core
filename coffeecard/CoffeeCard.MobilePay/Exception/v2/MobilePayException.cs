namespace CoffeeCard.MobilePay.Exception.v2
{
    public class MobilePayException : System.Exception
    {
        public int StatusCode { get; }
        
        public string? ErrorCode { get; }

        public MobilePayException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public MobilePayException(int statusCode, string message, string errorCode) : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}