using System.Collections.Generic;
using CoffeeCard.Models.DataTransferObjects.AppConfig;
using CoffeeCard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.Controllers
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
