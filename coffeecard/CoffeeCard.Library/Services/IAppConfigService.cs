using CoffeeCard.Common.Models.DataTransferObjects.AppConfig;

namespace CoffeeCard.Library.Services
{
    public interface IAppConfigService
    {
        AppConfigDto RetreiveConfiguration();
    }
}