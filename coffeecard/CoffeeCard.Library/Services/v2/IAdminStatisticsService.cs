using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.AdminStatistics;

namespace CoffeeCard.Library.Services.v2
{
    public interface IAdminStatisticsService
    {
        public Task<List<UnusedClipsResponse>> GetUsableClips(
            UnusedClipsRequest unusedClipsRequest
        );
    }
}
