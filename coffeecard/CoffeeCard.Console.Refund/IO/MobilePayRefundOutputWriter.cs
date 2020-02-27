using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CoffeeCard.Console.Refund.Model;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.Console.Refund.IO
{
    public class MobilePayRefundOutputWriter : IOutputWriter<IList<RefundResponse>>
    {
        private const string FileName = "output.txt";

        private readonly ILogger<MobilePayRefundOutputWriter> _log;

        public MobilePayRefundOutputWriter(ILogger<MobilePayRefundOutputWriter> log)
        {
            _log = log;
        }

        public async Task WriteToFileAsync(IList<RefundResponse> refunds)
        {
            await using var writer = new StreamWriter(FileName);
            foreach (var refund in refunds)
            {
                await writer.WriteLineAsync(refund.ToString());
            }

            _log.LogInformation("Refund results saved to {filename}", FileName);
        }
    }
}
