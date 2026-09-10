using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.Statistics;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2
{
    public interface IStatisticsService
    {
        Task<IEnumerable<QuickStatResponse>> GetQuickStatsAsync(User user);
    }
}
