using System.Net.Http;
using System.Threading.Tasks;

namespace coffeecard.Services
{
    public interface IMobilePayService
    {
        Task<HttpResponseMessage> CheckOrderIdAgainstMPBackendAsync(string orderId);
    }
}
