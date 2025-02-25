using System;
using System.Net.Http;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.MobilePay.Clients;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.MobilePay.Generated.Api.WebhooksApi;
using CoffeeCard.Models.DataTransferObjects.MobilePay;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.MobilePay.Service.v2;

public class MobilePayAccessTokenService : IMobilePayAccessTokenService
{

    private readonly AccessTokenClient _accessTokenClient;
    private readonly MobilePaySettingsV3 _settings;
    private readonly ILogger<MobilePayAccessTokenService> _logger;

    public MobilePayAccessTokenService(
        AccessTokenClient accessTokenClient,
        MobilePaySettingsV3 mobilePaySettings,
        ILogger<MobilePayAccessTokenService> logger
    )
    {
        _accessTokenClient = accessTokenClient;
        _settings = mobilePaySettings;
        _logger = logger;
    }
    public async Task<GetAuthorizationTokenResponse> GetAuthorizationTokenAsync()
    {
        try
        {
            var response = await _accessTokenClient.GetToken(_settings.ClientId.ToString(), _settings.ClientSecret);

            return new GetAuthorizationTokenResponse()
            {
                AccessToken = response.Access_token,
                ExpiresOn = DateTimeOffset.FromUnixTimeSeconds(long.Parse(response.Expires_on))
            };
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new MobilePayApiException(503, $"Unable to get access token: {ex.Message}");
        }
    }
}