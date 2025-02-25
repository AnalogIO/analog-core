using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.MobilePay.Generated.Api.AccessTokenApi;
using CoffeeCard.MobilePay.Service.v2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;

namespace CoffeeCard.MobilePay.Utils;

public class MobilePayAuthorizationDelegatingHandler(
    IMobilePayAccessTokenService accessTokenService,
    IMemoryCache memoryCache
    ) : DelegatingHandler
{
    private const string MpAccessTokenCacheKey = "MpAccessTokenKey";

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var accessToken = await GetAccessTokenAsync();

        request.Headers.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            accessToken);

        var httpResponseMessage = await base.SendAsync(
            request,
            cancellationToken);

        return httpResponseMessage;
    }

    private async Task<string?> GetAccessTokenAsync()
    {
        return await memoryCache.GetOrCreateAsync(MpAccessTokenCacheKey, async entry =>
        {
            var accessTokenResponse = await accessTokenService.GetAuthorizationTokenAsync();

            entry.AbsoluteExpiration = accessTokenResponse.ExpiresOn;
            return accessTokenResponse.AccessToken;
        });
    }
}