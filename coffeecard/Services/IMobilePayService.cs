using System.Net.Http;
using System.Threading.Tasks;

namespace CoffeeCard.Services
{
    public interface IMobilePayService
    {
        Task<HttpResponseMessage> CheckOrderIdAgainstMPBackendAsync(string orderId);
    }
}