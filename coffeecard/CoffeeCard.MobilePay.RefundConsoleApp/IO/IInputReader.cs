using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.RefundConsoleApp.Model;

namespace CoffeeCard.MobilePay.RefundConsoleApp.IO
{
    public interface IInputReader<T>
    {
        public Task<IEnumerable<CompletedOrder>> ReadFromCommaSeparatedFile(string path);
    }
}
