namespace CoffeeCard.Common.Errors
{
    public class ApiError
    {
        public ApiError(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
