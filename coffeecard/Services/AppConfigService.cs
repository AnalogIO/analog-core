using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coffeecard.Models.DataTransferObjects.AppConfig;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace coffeecard.Services
{
    public class AppConfigService : IAppConfigService
    {
        private readonly IConfiguration _configuration;

        public AppConfigService(IConfiguration configuration)
        {
            _configuration = configuration;          
        }
        //TODO throw exception if EnvironmentType and MerchantID is null or empty-string.
        public AppConfigDTO RetreiveConfiguration()
        {
            var _environmentType = _configuration["EnvironmentType"];
            var _merchantId = _configuration["MobilePayMerchantId"];
            if(string.IsNullOrEmpty(_environmentType)) throw new KeyNotFoundException();
            if(string.IsNullOrEmpty(_merchantId)) throw new KeyNotFoundException();
            return new AppConfigDTO
            {
                EnvironmentType = _environmentType,
                MerchantId = _merchantId
            };
        }
    }
}
