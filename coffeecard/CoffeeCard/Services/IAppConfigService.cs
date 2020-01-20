using CoffeeCard.Models.DataTransferObjects.AppConfig;

namespace CoffeeCard.Services
{
    public interface IAppConfigService
    {
        AppConfigDTO RetreiveConfiguration();
    }
}