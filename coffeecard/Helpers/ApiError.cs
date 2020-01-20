namespace coffeecard.Helpers
{
    public class ApiError
    {
        public string Message { get; set; }

        public ApiError(string message)
        {
            this.Message = message;
        }
    }
}
