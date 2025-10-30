using CoffeeCard.Common.Configuration;
using CoffeeCard.Models.DataTransferObjects.v2.AppConfig;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers.v2
{
    /// <summary>
    /// Controller for retrieving app configuration
    /// </summary>
    [ApiController]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/appconfig")]
    public class AppConfigController : ControllerBase
    {
        private readonly EnvironmentSettings _environmentSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigController"/> class.
        /// </summary>
        public AppConfigController(EnvironmentSettings environmentSettings)
        {
            _environmentSettings = environmentSettings;
        }

        /// <summary>
        /// Get app configuration
        /// </summary>
        /// <returns>App configuration</returns>
        /// <response code="200">Successful request</response>
        [HttpGet]
        [ProducesResponseType(typeof(AppConfig), StatusCodes.Status200OK)]
        public ActionResult<AppConfig> Get()
        {
            var environment = _environmentSettings.EnvironmentType;
            return Ok(new AppConfig { EnvironmentType = environment });
        }
    }
}
