using System.Threading.Tasks;

namespace CoffeeCard.MobilePay.RefundConsoleApp.IO
{
    public interface IOutputWriter<in T>
    {
        Task WriteToFileAsync(T refunds);
    }
}
