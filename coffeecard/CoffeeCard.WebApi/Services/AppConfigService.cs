using CoffeeCard.Common.Configuration;
using CoffeeCard.WebApi.Models.DataTransferObjects.AppConfig;
using Microsoft.Extensions.Options;

namespace CoffeeCard.WebApi.Services
{
    public class AppConfigService : IAppConfigService
    {
        private readonly EnvironmentSettings _environmentSettings;
        private readonly MobilePaySettings _mobilePaySettings;

        public AppConfigService(IOptions<MobilePaySettings> mobilePaySettings, EnvironmentSettings environmentSettings)
        {
            _mobilePaySettings = mobilePaySettings.Value;
            _environmentSettings = environmentSettings;
        }

        public AppConfigDTO RetreiveConfiguration()
        {
            var _environmentType = _environmentSettings.EnvironmentType;
            var _merchantId = _mobilePaySettings.MerchantId;

            return new AppConfigDTO
            {
                EnvironmentType = _environmentType,
                MerchantId = _merchantId
            };
        }
    }
}