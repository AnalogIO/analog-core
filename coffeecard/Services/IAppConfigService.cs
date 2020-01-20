using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.AppConfig;

namespace CoffeeCard.Services
{
    public interface IAppConfigService
    {
        AppConfigDTO RetreiveConfiguration();
    }
}
