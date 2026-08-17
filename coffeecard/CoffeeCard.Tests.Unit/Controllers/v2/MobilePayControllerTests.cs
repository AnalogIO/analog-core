using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.MobilePay.Generated.Api.ePaymentApi;
using CoffeeCard.WebApi.Controllers.v2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CoffeeCard.Tests.Unit.Controllers.v2;

public class MobilePayControllerTests
{
    [Fact]
    public async Task Webhook_ValidatesSignatureAgainstRawRequestBody()
    {
        const string rawBody =
            """{"msn":"123456","reference":"order-123","pspReference":"1234567890","name":"AUTHORIZED","amount":{"value":1000,"currency":"DKK"},"timestamp":"2026-08-17T13:54:49.1234567+00:00","success":true,"userDetails":{"email":"customer@example.com"}}""";
        const string secret = "webhook-secret";
        const string date = "Mon, 17 Aug 2026 13:54:49 GMT";
        const string host = "webhook.example";
        const string path = "/api/v2/mobilepay/webhook";
        const string query = "?attempt=1";

        var requestBody = Encoding.UTF8.GetBytes(rawBody);
        var contentHash = Convert.ToBase64String(SHA256.HashData(requestBody));
        var signedString = $"POST\n{path}{query}\n{date};{host};{contentHash}";
        using var hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var signature = Convert.ToBase64String(
            hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(signedString))
        );
        var authorization =
            $"HMAC-SHA256 SignedHeaders=x-ms-date;host;x-ms-content-sha256&Signature={signature}";

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Method = "POST";
        httpContext.Request.Path = path;
        httpContext.Request.QueryString = new QueryString(query);
        httpContext.Request.Host = new HostString(host);
        httpContext.Request.Body = new MemoryStream(requestBody);
        httpContext.Request.Body.Position = httpContext.Request.Body.Length;

        var purchaseServiceMock = new Mock<IPurchaseService>();
        purchaseServiceMock
            .Setup(service => service.HandleMobilePayPaymentUpdate(It.IsAny<WebhookEvent>()))
            .Returns(Task.CompletedTask);
        var webhookServiceMock = new Mock<IWebhookService>();
        webhookServiceMock.Setup(service => service.GetSignatureKey()).ReturnsAsync(secret);

        var controller = new MobilePayController(
            purchaseServiceMock.Object,
            webhookServiceMock.Object,
            new Mock<ILogger<MobilePayController>>().Object
        )
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
        };

        var result = await controller.Webhook(
            new WebhookEvent
            {
                Msn = "123456",
                Reference = "order-123",
                PspReference = "1234567890",
                Name = PaymentEventName.AUTHORIZED,
                Amount = new Amount { Value = 1000, Currency = Currency.DKK },
                Timestamp = DateTimeOffset.Parse("2026-08-17T13:54:49.1234567+00:00"),
                Success = true,
            },
            contentHash,
            date,
            authorization
        );

        Assert.IsType<NoContentResult>(result);
        purchaseServiceMock.Verify(
            service => service.HandleMobilePayPaymentUpdate(It.IsAny<WebhookEvent>()),
            Times.Once
        );
    }
}
