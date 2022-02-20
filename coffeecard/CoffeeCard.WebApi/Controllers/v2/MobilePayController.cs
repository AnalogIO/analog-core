using System.Threading.Tasks;
using CoffeeCard.Library.Services.v2;
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
        private readonly IPurchaseService _purchaseService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MobilePayController"/> class.
        /// </summary>
        /// <param name="purchaseService"></param>
        public MobilePayController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        /// <summary>
        /// Webhook to be invoked by MobilePay backend
        /// </summary>
        /// <param name="request">Webhook request</param>
        /// <param name="signature">Webhook signature</param>
        /// <response code="200">Webhook processed</response>
        [HttpPost("webhook")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<ActionResult> Webhook([FromBody] MobilePayWebhook request, [FromHeader(Name = "x-mobilepay-signature")] string signature)
        {
            // FIXME Validate Header

            Log.Information("MobilePay Webhook invoked. Request: {Request}", request);
            await _purchaseService.HandleMobilePayPaymentUpdate(request);

            return Ok();
        }
    }
}