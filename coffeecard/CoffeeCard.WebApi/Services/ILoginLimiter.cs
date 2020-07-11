using System;

namespace CoffeeCard.WebApi.Services
{
    public interface ILoginLimiter
    {
        (DateTime, int) UpdateAndGetLoginAttemptCount(string email);
        void RemoveEntry(string email);
    }
}