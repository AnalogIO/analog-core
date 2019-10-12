using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coffeecard.Models.DataTransferObjects.AppConfig;

namespace coffeecard.Services
{
    public interface IAppConfigService
    {
        AppConfigDTO RetreiveConfiguration();
    }
}
