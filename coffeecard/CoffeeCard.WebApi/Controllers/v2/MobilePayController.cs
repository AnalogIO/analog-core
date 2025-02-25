using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.MobilePay.Generated.Api.ePaymentApi;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        private readonly ILogger<MobilePayController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MobilePayController"/> class.
        /// </summary>
        public MobilePayController(IPurchaseService purchaseService, IWebhookService webhookService,
            MobilePaySettingsV2 mobilePaySettings, ILogger<MobilePayController> logger)
        {
            _purchaseService = purchaseService;
            _webhookService = webhookService;
            _mobilePaySettings = mobilePaySettings;
            _logger = logger;
        }

        /// <summary>
        /// Webhook to be invoked by MobilePay backend
        /// </summary>
        /// <param name="request">Webhook request</param>
        /// <param name="mpSignatureHeader">Webhook signature</param>
        /// <param name="dateHeader"></param>
        /// <param name="authorizationHeader"></param>
        /// <response code="204">Webhook processed</response>
        /// <response code="400">Signature is not valid</response>
        [HttpPost("webhook")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Webhook(
            [FromBody] WebhookEvent request,
            [FromHeader(Name = "x-ms-content-sha256")] string mpSignatureHeader,
            [FromHeader(Name = "x-ms-date")] string dateHeader,
            [FromHeader(Name = "Authorization")] string authorizationHeader
        )
        {
            _logger.LogInformation($"Received webhook request: {request}");
            _logger.LogInformation($"Received headers: {mpSignatureHeader}, {dateHeader}, {authorizationHeader}");

            // Step 1: Verify content hash
            // var requestContent = JsonConvert.SerializeObject(request);
            // var contentHashInBytes = SHA256.HashData(Encoding.UTF8.GetBytes(requestContent));
            // var computedContentHash = Convert.ToBase64String(contentHashInBytes);
            //
            // if (computedContentHash != mpSignatureHeader)
            // {
            //     _logger.LogWarning("Content hash did not match the computed hash for request '{Request}', header hash: {Signature}",
            //         request, mpSignatureHeader);
            //     return BadRequest("Content hash is not valid");
            // }

            // Step 2: Verify HMAC signature
            // var secret = await _webhookService.GetSignatureKey();
            // var requestPath = Request.Path + Request.QueryString;
            // var expectedSignedString = $"POST\n{requestPath}\n{dateHeader}\n{Request.Host}\n{computedContentHash}";
            //
            // using (var hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            // {
            //     var hmacSha256Bytes = Encoding.UTF8.GetBytes(expectedSignedString);
            //     var hmacSha256Hash = hmacSha256.ComputeHash(hmacSha256Bytes);
            //     var computedSignature = Convert.ToBase64String(hmacSha256Hash);
            //     var expectedAuthorization = $"HMAC-SHA256 SignedHeaders=x-ms-date;host;x-ms-content-sha256&Signature={computedSignature}";
            //
            //     if (expectedAuthorization != authorizationHeader)
            //     {
            //         _logger.LogWarning("Signature did not match the computed signature for request '{Request}', header signature: {Signature}",
            //             request, authorizationHeader);
            //         return BadRequest("Signature is not valid");
            //     }
            // }

            _logger.LogInformation("MobilePay Webhook invoked with request: '{Request}'", request);
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

            _logger.LogDebug("Raw request body (trimmed): '{Body}'", rawRequestBody.Trim());
            _logger.LogDebug("Endpoint Url '{EndpointUrl}', SignatureKey '{Signature}'", endpointUrl, signatureKey);

            var hash = new HMACSHA1(Encoding.UTF8.GetBytes(signatureKey))
                .ComputeHash(Encoding.UTF8.GetBytes(endpointUrl + rawRequestBody.Trim()));
            var computedSignature = Convert.ToBase64String(hash);

            _logger.LogDebug("ComputedSignature: {Signature}", computedSignature);
            _logger.LogDebug("mpSignatureHeader {mpSignatureHeader}", mpSignatureHeader);

            return mpSignatureHeader.Equals(computedSignature);
        }
    }
}