using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.Service.v2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;

namespace CoffeeCard.MobilePay.Utils;

public class MobilePayAuthorizationDelegatingHandler(
    IMobilePayAccessTokenService accessTokenService
) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var accessTokenResponse = await accessTokenService.GetAuthorizationTokenAsync();

        request.Headers.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            accessTokenResponse.AccessToken
        );

        var httpResponseMessage = await base.SendAsync(request, cancellationToken);

        return httpResponseMessage;
    }
}
