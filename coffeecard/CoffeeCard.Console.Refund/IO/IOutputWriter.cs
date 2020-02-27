using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeCard.Console.Refund.IO
{
    public interface IOutputWriter<T>
    {
        Task WriteToFileAsync(T t);
    }
}
