using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.MobilePay.Generated.Api.AccessTokenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;

namespace CoffeeCard.MobilePay.Utils;

public class MobilePayAuthorizationDelegatingHandler(
    MobilePaySettingsV3 mobilePaySettingsV3,
    AccessTokenApi accessTokenApi,
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

        httpResponseMessage.EnsureSuccessStatusCode();

        return httpResponseMessage;
    }

    private async Task<string?> GetAccessTokenAsync()
    {
        return await memoryCache.GetOrCreateAsync(MpAccessTokenCacheKey, async entry =>
        {
            var accessTokenResponse = await accessTokenApi.FetchAuthorizationTokenUsingPostAsync(
                mobilePaySettingsV3.ClientId,
                mobilePaySettingsV3.ClientSecret,
                mobilePaySettingsV3.OcpApimSubscriptionKey,
                mobilePaySettingsV3.MerchantSerialNumber,
                mobilePaySettingsV3.VippsSystemName,
                mobilePaySettingsV3.VippsSystemVersion,
                mobilePaySettingsV3.VippsSystemPluginName,
                mobilePaySettingsV3.VippsSystemPluginVersion);

            entry.AbsoluteExpiration = DateTimeOffset.FromUnixTimeSeconds(long.Parse(accessTokenResponse.Expires_on));
            return accessTokenResponse.Access_token;
        });
    }
}