using CoffeeCard.Common.Configuration;
using CoffeeCard.Models.DataTransferObjects.AppConfig;
using Microsoft.Extensions.Options;

namespace CoffeeCard.Services
{
    public class AppConfigService : IAppConfigService
    {
        private readonly MobilePaySettings _mobilePaySettings;
        private readonly EnvironmentSettings _environmentSettings;

        public AppConfigService(IOptions<MobilePaySettings> mobilePaySettings, EnvironmentSettings environmentSettings)
        {
            _mobilePaySettings = mobilePaySettings.Value;
            _environmentSettings = environmentSettings;
        }

        public AppConfigDTO RetreiveConfiguration()
        {
            var _environmentType = _environmentSettings.EnvironmentType;
            var _merchantId =_mobilePaySettings.MerchantId;

            return new AppConfigDTO
            {
                EnvironmentType = _environmentType,
                MerchantId = _merchantId
            };
        }
    }
}