using CoffeeCard.WebApi.Models;

namespace CoffeeCard.WebApi.Services
{
    public interface ILoginLimiter
    {
        bool LoginAllowed(User user);
        void ResetLoginAttemptsForUser(User user);
    }
}