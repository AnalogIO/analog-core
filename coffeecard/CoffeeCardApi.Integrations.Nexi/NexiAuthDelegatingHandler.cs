using CoffeeCard.Common.Configuration;

namespace CoffeeCardApi.Integrations.Nexi;

public class NexiAuthDelegatingHandler : DelegatingHandler
{
    private readonly NexiSettings _settings;

    public NexiAuthDelegatingHandler(NexiSettings settings)
    {
        _settings = settings;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        // Nexi expects no scheme stated for the auth header
        request.Headers.Add("Authorization", _settings.ApiKey);
        return base.SendAsync(request, cancellationToken);
    }
}
