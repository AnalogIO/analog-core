using System;
using CoffeeCard.Models.DataTransferObjects.AppConfig;
using Microsoft.Extensions.Configuration;

namespace CoffeeCard.Services
{
    public class AppConfigService : IAppConfigService
    {
        private readonly IConfiguration _configuration;

        public AppConfigService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AppConfigDTO RetreiveConfiguration()
        {
            var _environmentType = _configuration["EnvironmentType"];
            var _merchantId = _configuration["MPMerchantID"];
            if (string.IsNullOrEmpty(_environmentType) || string.IsNullOrEmpty(_merchantId))
                throw new ArgumentNullException();
            return new AppConfigDTO
            {
                EnvironmentType = _environmentType,
                MerchantId = _merchantId
            };
        }
    }
}