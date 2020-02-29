using CoffeeCard.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers
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
        public IActionResult Get()
        {
            var appConfig = _appConfigService.RetreiveConfiguration();
            return Ok(appConfig);
        }
    }
}