using System;
using System.Net.Http;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.MobilePay.Clients;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.MobilePay.Generated.Api.WebhooksApi;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.MobilePay.Service.v2;

public class MobilePayAccessTokenService(
    IAccessTokenClient accessTokenClient,
    MobilePaySettings mobilePaySettings,
    IMemoryCache memoryCache
) : IMobilePayAccessTokenService
{
    private const string MpAccessTokenCacheKey = "MpAccessTokenKey";

    public async Task<GetAuthorizationTokenResponse> GetAuthorizationTokenAsync()
    {
        var tokenResponse = await memoryCache.GetOrCreateAsync(
            MpAccessTokenCacheKey,
            async entry =>
            {
                var clientResponse = await accessTokenClient.GetToken(
                    mobilePaySettings.ClientId.ToString(),
                    mobilePaySettings.ClientSecret
                );
                var tokenResponse = new GetAuthorizationTokenResponse()
                {
                    AccessToken = clientResponse.Access_token,
                    ExpiresOn = DateTimeOffset.FromUnixTimeSeconds(
                        long.Parse(clientResponse.Expires_on)
                    ),
                };

                entry.AbsoluteExpiration = tokenResponse.ExpiresOn - TimeSpan.FromMinutes(5);
                return tokenResponse;
            }
        );

        return tokenResponse!;
    }
}
