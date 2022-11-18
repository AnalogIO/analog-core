namespace CoffeeCard.Common.Errors
{
    public class EntityNotFoundException : ApiException
    {
        public EntityNotFoundException(string message) : base(message, statusCode: 404)
        {
        }
    }
}