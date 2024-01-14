using System;

namespace CoffeeCard.Library.Utils
{
    public interface IDateTimeProvider
    {
        public DateTime UtcNow();
    }
}
