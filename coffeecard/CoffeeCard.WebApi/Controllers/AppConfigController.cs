using System;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.DataTransferObjects.AppConfig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers
{
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
        public AppConfigController()
        {
        }

        /// <summary>
        /// Get app configuration
        /// </summary>
        /// <returns>App configuration</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpGet]
        [ProducesResponseType(typeof(AppConfigDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public ActionResult<AppConfigDto> Get()
        {
            return StatusCode(StatusCodes.Status410Gone);
        }
    }
}