using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Console.Refund.Model;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.Console.Refund.IO
{
    public class CompletedOrderInputReader : IInputReader<CompletedOrder>
    {
        private readonly ILogger<CompletedOrderInputReader> _log;

        public CompletedOrderInputReader(ILogger<CompletedOrderInputReader> log)
        {
            _log = log;
        }

        public async Task<IEnumerable<CompletedOrder>> ReadFromCommaSeparatedFile(string path)
        {
            try
            {
                string lines;
                using (var reader = new StreamReader(path))
                {
                    lines = await reader.ReadToEndAsync();
                }

                var completedOrders = ParseInputToTransactions(lines);
                _log.LogInformation("{size} MobilePay transactions read from {path}", completedOrders.Count(), path);

                return completedOrders;
            }
            catch (IOException ex)
            {
                _log.LogError("Error reading file. {ex}", ex);
                return new List<CompletedOrder>();
            }
        }

        private IList<CompletedOrder> ParseInputToTransactions(string lines)
        {
            var splitLine = lines.Split(",");

            var completedOrders = new List<CompletedOrder>();
            foreach (var line in splitLine)
            {
                completedOrders.Add(new CompletedOrder
                {
                    OrderId = line.Trim()
                });
            }

            return completedOrders;
        }
    }
}
