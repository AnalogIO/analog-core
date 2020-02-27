using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Console.Refund.Model;

namespace CoffeeCard.Console.Refund.IO
{
    public interface IInputReader<T>
    {
        public Task<IEnumerable<CompletedOrder>> ReadFromCommaSeparatedFile(string path);
    }
}
