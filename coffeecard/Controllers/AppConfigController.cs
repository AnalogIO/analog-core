using coffeecard.Models.DataTransferObjects.AppConfig;
using coffeecard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeecard.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiController]
    public class AppConfigController : ControllerBase
    {
        private readonly IAppConfigService _appConfigService;

        public AppConfigController(IAppConfigService appConfigService)
        {
            _appConfigService = appConfigService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<AppConfigDTO>> Get()
        {
            var appConfig = _appConfigService.RetreiveConfiguration();
            return Ok(appConfig);
        }
    }
}
