using CoffeeCard.Common.Configuration;

namespace CoffeeCardApi.Integrations.Nexi;

public class NexiAuthDelegatingHandler : DelegatingHandler
{
    private readonly NexiSettings _settings;

    public NexiAuthDelegatingHandler(NexiSettings settings)
    {
        _settings = settings;
    }

    protected override HttpResponseMessage Send(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        ApplyNexiAuthHeader(request);
        return base.Send(request, cancellationToken);
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        ApplyNexiAuthHeader(request);
        return base.SendAsync(request, cancellationToken);
    }

    private void ApplyNexiAuthHeader(HttpRequestMessage request)
    {
        // Nexi expects no scheme stated for the auth header
        request.Headers.Add("Authorization", _settings.ApiKey);
    }
}
