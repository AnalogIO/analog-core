using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.MobilePay.Generated.Api.ePaymentApi;
using CoffeeCard.Models.DataTransferObjects.MobilePay;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSwag.Annotations;

namespace CoffeeCard.WebApi.Controllers.v2;

/// <summary>
///     Controller exposing Webhook endpoints for MobilePay
/// </summary>
[ApiController]
[ApiVersion("2")]
[Route("api/v{version:apiVersion}/mobilepay")]
public class MobilePayController : ControllerBase
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ",
        DateTimeZoneHandling = DateTimeZoneHandling.Utc,
    };

    private readonly ILogger<MobilePayController> _logger;
    private readonly IPurchaseService _purchaseService;
    private readonly IWebhookService _webhookService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MobilePayController" /> class.
    /// </summary>
    public MobilePayController(
        IPurchaseService purchaseService,
        IWebhookService webhookService,
        ILogger<MobilePayController> logger
    )
    {
        _purchaseService = purchaseService;
        _webhookService = webhookService;
        _logger = logger;
    }

    /// <summary>
    ///     Webhook to be invoked by MobilePay backend
    /// </summary>
    /// <param name="request">Webhook request</param>
    /// <param name="contentShaHeader">Webhook content hash</param>
    /// <param name="dateHeader">Webhook date header</param>
    /// <param name="authorizationHeader">Webhook HMAC authorization signature</param>
    /// <response code="204">Webhook processed</response>
    /// <response code="400">Signature is not valid</response>
    [HttpPost("webhook")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [OpenApiIgnore]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> Webhook(
        [FromBody] WebhookEvent request,
        [FromHeader(Name = "x-ms-content-sha256")] string contentShaHeader,
        [FromHeader(Name = "x-ms-date")] string dateHeader,
        [FromHeader(Name = "Authorization")] string authorizationHeader
    )
    {
        var json = JsonConvert.SerializeObject(request, JsonSerializerSettings);

        var signatureIsValid = await VerifySignature(
            json,
            contentShaHeader,
            dateHeader,
            authorizationHeader
        );
        if (!signatureIsValid)
        {
            _logger.LogWarning(
                "Could not verify signature for request with reference: {reference}",
                request.Reference
            );
            return BadRequest("Could not verify signature");
        }

        _logger.LogInformation("MobilePay Webhook invoked with request: '{@Request}'", request);
        await _purchaseService.HandleMobilePayPaymentUpdate(request);

        return NoContent();
    }

    private async Task<bool> VerifySignature(
        string requestJson,
        string contentShaHeader,
        string dateHeader,
        string authorizationHeader
    )
    {
        var contentHashInBytes = SHA256.HashData(Encoding.UTF8.GetBytes(requestJson));
        var computedContentHash = Convert.ToBase64String(contentHashInBytes);

        if (!computedContentHash.Equals(contentShaHeader, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(
                "Content hash did not match the computed hash for, header hash: {ContentSha}",
                contentShaHeader
            );
            return false;
        }

        var secret = await _webhookService.GetSignatureKey();
        var requestPath = Request.Path + Request.QueryString;

        var expectedSignedString =
            $"{Request.Method}\n"
            + $"{requestPath}\n"
            + $"{dateHeader};{Request.Host};{contentShaHeader}";

        using var hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hmacSha256Bytes = Encoding.UTF8.GetBytes(expectedSignedString);
        var hmacSha256Hash = hmacSha256.ComputeHash(hmacSha256Bytes);
        var computedSignature = Convert.ToBase64String(hmacSha256Hash);
        var expectedAuthorization =
            $"HMAC-SHA256 SignedHeaders=x-ms-date;host;x-ms-content-sha256&Signature={computedSignature}";

        if (!expectedAuthorization.Equals(authorizationHeader, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(
                "Signature did not match the computed signature, header signature: {AuthorizationHeader}",
                authorizationHeader
            );
            return false;
        }

        return true;
    }
}
