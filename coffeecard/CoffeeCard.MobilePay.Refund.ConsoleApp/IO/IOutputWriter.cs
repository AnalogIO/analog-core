using System.Threading.Tasks;

namespace CoffeeCard.MobilePay.Refund.ConsoleApp.IO
{
    public interface IOutputWriter<in T>
    {
        Task WriteToFileAsync(T refunds);
    }
}