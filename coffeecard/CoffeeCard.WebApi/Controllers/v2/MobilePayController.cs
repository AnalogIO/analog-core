using System;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CoffeeCard.WebApi.Controllers.v2
{
    /// <summary>
    /// Controller exposing Webhook endpoints for MobilePay
    /// </summary>
    [ApiController]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/mobilepay")]
    public class MobilePayController : ControllerBase
    {
        /// <summary>
        /// Webhook to be invoked by MobilePay backend
        /// </summary>
        /// <param name="request">Webhook request</param>
        /// <response code="200">Webhook processed</response>
        [HttpPost("webhook")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<ActionResult> Webhook(MobilePayWebhook request)
        {
            Log.Information("MobilePay Webhook invoked. Request={Request}", request);

            throw new NotImplementedException();
        }
    }
}