using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services
{
    public interface ILoginLimiter
    {
        bool LoginAllowed(User user);
        void ResetLoginAttemptsForUser(User user);
    }
}
