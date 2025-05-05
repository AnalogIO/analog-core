using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;

namespace CoffeeCard.MobilePay.Service.v2;

public interface IMobilePayAccessTokenService
{
    Task<GetAuthorizationTokenResponse> GetAuthorizationTokenAsync();
}
