using CoffeeCard.WebApi.Models.DataTransferObjects.AppConfig;

namespace CoffeeCard.WebApi.Services
{
    public interface IAppConfigService
    {
        AppConfigDto RetreiveConfiguration();
    }
}