using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers
{
    // TODO: Remove this Controller after June 2023

    /// <summary>
    /// Controller for retrieving app configuration
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Obsolete(message: "Replaced by AppConfig v2")]
    [Authorize]
    public class AppConfigController : ControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigController"/> class.
        /// </summary>
        public AppConfigController() { }

        /// <summary>
        /// Get app configuration
        /// </summary>
        /// <returns>App configuration</returns>
        /// <response code="410">Deprecated</response>
        [HttpGet]
        [Obsolete(message: "Replaced by AppConfig v2")]
        [ProducesResponseType(typeof(void), StatusCodes.Status410Gone)]
        public ActionResult Get()
        {
            return StatusCode(StatusCodes.Status410Gone);
        }
    }
}
