using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers
{
    /// <summary>
    /// Controller for pinging the webservice for heartbeat checks
    /// </summary>
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        /// <summary>
        /// Ping
        /// </summary>
        /// <returns>pong</returns>
        /// <response code="200">Successful request</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Ping()
        {
            return Ok("pong");
        }
    }
}