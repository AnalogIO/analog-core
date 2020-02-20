namespace CoffeeCard.Helpers
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