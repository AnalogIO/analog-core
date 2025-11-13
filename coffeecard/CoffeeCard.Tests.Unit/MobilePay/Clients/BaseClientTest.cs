using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace CoffeeCard.Tests.Unit.MobilePay.Clients;

public abstract class BaseClientTest
{
    private const string BaseUrl = "https://test.notvipps.dk";

    protected static HttpClient CreateMockHttpClient<T>(HttpStatusCode statusCode, T content)
    {
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                (HttpRequestMessage request, CancellationToken token) =>
                {
                    // Create a response with the RequestMessage property set
                    var response = new HttpResponseMessage
                    {
                        StatusCode = statusCode,
                        Content = JsonContent.Create(content),
                        RequestMessage = request, // Set the RequestMessage to avoid NullReferenceException
                    };

                    return response;
                }
            );

        var httpClient = new HttpClient(handlerMock.Object) { BaseAddress = new Uri(BaseUrl) };

        return httpClient;
    }
}
