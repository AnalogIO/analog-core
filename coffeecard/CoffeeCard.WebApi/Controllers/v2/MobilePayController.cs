using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
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
        private readonly MobilePaySettingsV2 _mobilePaySettings;
        private readonly IWebhookService _webhookService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MobilePayController"/> class.
        /// </summary>
        public MobilePayController(IPurchaseService purchaseService, IWebhookService webhookService, MobilePaySettingsV2 mobilePaySettings)
        {
            _purchaseService = purchaseService;
            _webhookService = webhookService;
            _mobilePaySettings = mobilePaySettings;
        }

        /// <summary>
        /// Webhook to be invoked by MobilePay backend
        /// </summary>
        /// <param name="request">Webhook request</param>
        /// <param name="signature">Webhook signature</param>
        /// <response code="204">Webhook processed</response>
        /// <response code="400">Signature is not valid</response>
        [HttpPost("webhook")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Webhook([FromBody] MobilePayWebhook request, [FromHeader(Name = "x-mobilepay-signature")] string signature)
        {
            var isSignatureValid = await VerifySignature(signature);
            if (!isSignatureValid)
            {
                Log.Error("Signature did not match the computed signature. Request Body: {Request} Signature: {Signature}", request, signature);
                return BadRequest("Signature is not valid");
            }
            
            Log.Information("MobilePay Webhook invoked. Request: {Request}", request);
            await _purchaseService.HandleMobilePayPaymentUpdate(request);

            return NoContent();
        }

        private async Task<bool> VerifySignature(string requestSignature)
        {
            if (!Request.Body.CanSeek)
            {
                Request.EnableBuffering();
            }
            
            Request.Body.Position = 0;
            
            var rawRequestBody = await new StreamReader(Request.Body).ReadToEndAsync().ConfigureAwait(false);
            
            Request.Body.Position = 0;
            
            Log.Information("Body: '{Body}'", rawRequestBody);

            var hash = new HMACSHA1(Encoding.UTF8.GetBytes(await _webhookService.SignatureKey()))
                .ComputeHash(Encoding.UTF8.GetBytes(_mobilePaySettings.WebhookUrl + rawRequestBody.Trim()));
            var computedSignature = Convert.ToBase64String(hash);

            Log.Information("ComputedSignature: {Signature}", computedSignature);
            
            return requestSignature.Equals(computedSignature);
        }
    }
}