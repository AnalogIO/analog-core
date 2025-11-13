using System.Threading.Tasks;

namespace CoffeeCard.Library.Services.v2
{
    public interface IStatisticService
    {
        public Task IncreaseStatisticsBy(int userId, int increaseBy);
    }
}
