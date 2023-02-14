using CoffeeCard.Models.DataTransferObjects.AppConfig;

namespace CoffeeCard.Library.Services
{
    public interface IAppConfigService
    {
        AppConfigDto RetreiveConfiguration();
    }
}