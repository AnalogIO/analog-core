using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace CoffeeCard.WebApi.Controllers.v2
{
    /// <summary>
    /// Controller exposing Webhook endpoints for MobilePay
    /// </summary>
    [ApiController]
    [ApiVersion("2")]
    [Route("api/v2/mobilepay")]
    public class MobilePayController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;
        private readonly MobilePaySettingsV2 _mobilePaySettings;
        private readonly IWebhookService _webhookService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MobilePayController"/> class.
        /// </summary>
        public MobilePayController(IPurchaseService purchaseService, IWebhookService webhookService,
            MobilePaySettingsV2 mobilePaySettings)
        {
            _purchaseService = purchaseService;
            _webhookService = webhookService;
            _mobilePaySettings = mobilePaySettings;
        }

        /// <summary>
        /// Webhook to be invoked by MobilePay backend
        /// </summary>
        /// <param name="request">Webhook request</param>
        /// <param name="mpSignatureHeader">Webhook signature</param>
        /// <response code="204">Webhook processed</response>
        /// <response code="400">Signature is not valid</response>
        [HttpPost("webhook")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Webhook([FromBody]MobilePayWebhook request, [FromHeader(Name = "x-mobilepay-signature")] [Required] string mpSignatureHeader)
        {
            var isSignatureValid = await VerifySignature(mpSignatureHeader);
            if (!isSignatureValid)
            {
                Log.Warning("Signature did not match the computed signature for request '{Request}', header signature: {Signature}",
                    request, mpSignatureHeader);
                return BadRequest("Signature is not valid");
            }

            Log.Information("MobilePay Webhook invoked with request: '{Request}'", request);
            await _purchaseService.HandleMobilePayPaymentUpdate(request);

            return NoContent();
        }

        private async Task<bool> VerifySignature(string mpSignatureHeader)
        {
            var endpointUrl = _mobilePaySettings.WebhookUrl;
            var signatureKey = await _webhookService.GetSignatureKey();

            if (!Request.Body.CanSeek)
            {
                Request.EnableBuffering();
            }

            HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);

            string rawRequestBody;
            using (var stream = new StreamReader(HttpContext.Request.Body))
            {
                rawRequestBody = await stream.ReadToEndAsync();
            }

            Log.Debug("Raw request body (trimmed): '{Body}'", rawRequestBody.Trim());
            Log.Debug("Endpoint Url '{EndpointUrl}', SignatureKey '{Signature}'", endpointUrl, signatureKey);
            
            var hash = new HMACSHA1(Encoding.UTF8.GetBytes(signatureKey))
                .ComputeHash(Encoding.UTF8.GetBytes(endpointUrl + rawRequestBody.Trim()));
            var computedSignature = Convert.ToBase64String(hash);
            
            Log.Debug("ComputedSignature: {Signature}", computedSignature);
            Log.Debug("mpSignatureHeader {mpSignatureHeader}", mpSignatureHeader);

            return mpSignatureHeader.Equals(computedSignature);
        }
    }
}