namespace coffeecard.Helpers
{
    public class ApiError
    {
        public string message { get; set; }

        public ApiError(string message)
        {
            this.message = message;
        }
    }
}
