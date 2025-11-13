using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CoffeeCard.MobilePay.Utils;

public class MobilePayIdempotencyDelegatingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

        var httpResponseMessage = await base.SendAsync(request, cancellationToken);

        return httpResponseMessage;
    }
}
