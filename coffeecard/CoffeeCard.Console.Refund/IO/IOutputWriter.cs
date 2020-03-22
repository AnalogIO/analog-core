using System.Threading.Tasks;

namespace CoffeeCard.Console.Refund.IO
{
    public interface IOutputWriter<in T>
    {
        Task WriteToFileAsync(T refunds);
    }
}