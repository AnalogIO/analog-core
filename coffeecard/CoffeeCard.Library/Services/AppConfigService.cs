using CoffeeCard.Common.Configuration;
using CoffeeCard.Models.DataTransferObjects.AppConfig;
using Microsoft.Extensions.Options;

namespace CoffeeCard.Library.Services
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

        public AppConfigDto RetreiveConfiguration()
        {
            var _environmentType = _environmentSettings.EnvironmentType;
            var _merchantId = _mobilePaySettings.MerchantId;

            return new AppConfigDto
            {
                EnvironmentType = _environmentType,
                MerchantId = _merchantId
            };
        }
    }
}