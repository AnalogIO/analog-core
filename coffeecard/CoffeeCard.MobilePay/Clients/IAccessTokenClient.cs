using System.Threading.Tasks;
using CoffeeCard.MobilePay.Generated.Api.AccessTokenApi;

namespace CoffeeCard.MobilePay.Clients;

public interface IAccessTokenClient
{
    public Task<AuthorizationTokenResponse> GetToken(string clientId, string clientSecret);
}
