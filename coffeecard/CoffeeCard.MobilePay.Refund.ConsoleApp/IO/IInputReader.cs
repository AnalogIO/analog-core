using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.Refund.ConsoleApp.Model;

namespace CoffeeCard.MobilePay.Refund.ConsoleApp.IO
{
    public interface IInputReader<T>
    {
        public Task<IEnumerable<CompletedOrder>> ReadFromCommaSeparatedFile(string path);
    }
}