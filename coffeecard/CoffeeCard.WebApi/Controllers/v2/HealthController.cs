using System;
using System.Threading.Tasks;
using CoffeeCard.Library.Persistence;
using CoffeeCard.MobilePay.Service.v2;
using CoffeeCard.Models.DataTransferObjects.v2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CoffeeCard.WebApi.Controllers.v2
{
    /// <summary>
    /// Controller for health checks
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="HealthController"/> class.
    /// </remarks>
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/health")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "apikey")]
    public class HealthController(IMobilePayPaymentsService mobilePayPaymentsService, CoffeeCardContext context) : ControllerBase
    {
        private readonly CoffeeCardContext _context = context;
        private readonly IMobilePayPaymentsService _mobilePayPaymentsService = mobilePayPaymentsService;

        /// <summary>
        /// Ping
        /// </summary>
        /// <returns>pong</returns>
        /// <response code="200">Successful request</response>
        [HttpGet("ping")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        /// <summary>
        /// Check service health
        /// </summary>
        /// <returns>pong</returns>
        /// <response code="200">Healthy service</response>
        /// <response code="503">Unhealthy service</response>
        [HttpGet("check")]
        [ProducesResponseType(typeof(ServiceHealthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceHealthResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Healthcheck()
        {
            var databaseConnected = await IsServiceCallSuccessful(async () => await _context.Database.CanConnectAsync());
            var mobilepayApiConnected = await IsServiceCallSuccessful(async () => await _mobilePayPaymentsService.GetPaymentPoints());

            var response = new ServiceHealthResponse()
            {
                Database = databaseConnected,
                MobilePay = mobilepayApiConnected
            };

            if (databaseConnected && mobilepayApiConnected)
            {
                return Ok(response);
            }

            return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
        }

        private static async Task<bool> IsServiceCallSuccessful(Func<Task> action)
        {
            try
            {
                await action();
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "Health check failed");
                return false;
            }
        }
    }
}