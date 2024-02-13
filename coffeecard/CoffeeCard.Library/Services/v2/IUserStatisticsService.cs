using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.UserStatistics;

namespace CoffeeCard.Library.Services.v2
{
    public interface IUserStatisticsService
    {
        public Task<List<UnusedClipsResponse>> GetUnusedClips(UnusedClipsRequest unusedClipsRequest);
    }
}