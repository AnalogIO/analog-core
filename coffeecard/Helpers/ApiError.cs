namespace CoffeeCard.Helpers
{
    public class ApiError
    {
        public ApiError(string message)
        {
            this.message = message;
        }

        public string message { get; set; }
    }
}