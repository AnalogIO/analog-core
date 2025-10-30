using System;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.MobilePay.Clients;
using CoffeeCard.MobilePay.Generated.Api.AccessTokenApi;
using CoffeeCard.MobilePay.Service.v2;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace CoffeeCard.Tests.Unit.MobilePay.Service
{
    public class MobilePayAccessTokenServiceTest
    {
        private readonly Mock<IAccessTokenClient> _accessTokenClientMock = new();
        private readonly MobilePaySettings _mobilePaySettings = new()
        {
            ClientId = Guid.NewGuid(),
            ClientSecret = "test-client-secret",
        };
        private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        [Fact(DisplayName = "GetAuthorizationTokenAsync returns token from client")]
        public async Task GetAuthorizationTokenAsync_ReturnsToken_FromClient()
        {
            // Arrange
            var expiresOn = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds().ToString();
            var expectedToken = new AuthorizationTokenResponse
            {
                Access_token = "test-access-token",
                Token_type = "Bearer",
                Expires_in = "3600",
                Expires_on = expiresOn,
            };

            _accessTokenClientMock
                .Setup(x =>
                    x.GetToken(
                        _mobilePaySettings.ClientId.ToString(),
                        _mobilePaySettings.ClientSecret
                    )
                )
                .ReturnsAsync(expectedToken);

            var service = new MobilePayAccessTokenService(
                _accessTokenClientMock.Object,
                _mobilePaySettings,
                _memoryCache
            );

            // Act
            var result = await service.GetAuthorizationTokenAsync();

            // Assert
            Assert.Equal(expectedToken.Access_token, result.AccessToken);
            Assert.Equal(
                DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiresOn)),
                result.ExpiresOn
            );

            // Verify client was called
            _accessTokenClientMock.Verify(
                x =>
                    x.GetToken(
                        _mobilePaySettings.ClientId.ToString(),
                        _mobilePaySettings.ClientSecret
                    ),
                Times.Once
            );
        }

        [Fact(DisplayName = "GetAuthorizationTokenAsync caches token")]
        public async Task GetAuthorizationTokenAsync_CachesToken()
        {
            // Arrange
            var expiresOn = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds().ToString();
            var expectedToken = new AuthorizationTokenResponse
            {
                Access_token = "test-access-token",
                Token_type = "Bearer",
                Expires_in = "3600",
                Expires_on = expiresOn,
            };

            _accessTokenClientMock
                .Setup(x =>
                    x.GetToken(
                        _mobilePaySettings.ClientId.ToString(),
                        _mobilePaySettings.ClientSecret
                    )
                )
                .ReturnsAsync(expectedToken);

            var service = new MobilePayAccessTokenService(
                _accessTokenClientMock.Object,
                _mobilePaySettings,
                _memoryCache
            );

            // Act
            var result1 = await service.GetAuthorizationTokenAsync();
            var result2 = await service.GetAuthorizationTokenAsync();

            // Assert
            Assert.Equal(expectedToken.Access_token, result1.AccessToken);
            Assert.Equal(expectedToken.Access_token, result2.AccessToken);

            // Verify client was called only once due to caching
            _accessTokenClientMock.Verify(
                x =>
                    x.GetToken(
                        _mobilePaySettings.ClientId.ToString(),
                        _mobilePaySettings.ClientSecret
                    ),
                Times.Once
            );
        }
    }
}
