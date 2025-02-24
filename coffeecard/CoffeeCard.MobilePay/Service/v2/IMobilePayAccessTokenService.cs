using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.MobilePay;

namespace CoffeeCard.MobilePay.Service.v2;

public interface IMobilePayAccessTokenService
{
    Task<GetAuthorizationTokenResponse> GetAuthorizationTokenAsync();
}