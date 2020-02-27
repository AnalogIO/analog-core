using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Console.Refund.IO;
using CoffeeCard.Console.Refund.Model;
using CoffeeCard.MobilePay.Exception;
using CoffeeCard.MobilePay.Service;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.Console.Refund.Handler
{
    public class RefundHandler
    {
        private readonly ILogger<RefundHandler> _log;
        private readonly IMobilePayService _mobilePayService;
        private readonly IOutputWriter<IList<RefundResponse>> _outputWriter;

        public RefundHandler(ILogger<RefundHandler> log, IOutputWriter<IList<RefundResponse>> outputWriter,
            IMobilePayService mobilePayService)
        {
            _log = log;
            _mobilePayService = mobilePayService;
            _outputWriter = outputWriter;
        }

        public async Task RefundPayments(IEnumerable<CompletedOrder> completedOrders)
        {
            var results = new List<RefundResponse>();
            foreach (var completedOrder in completedOrders)
            {
                try
                {
                    var result = await _mobilePayService.RefundPayment(completedOrder.OrderId);
                    _log.LogInformation("SUCCESS Refunded order={result}", result);

                    results.Add(new RefundResponse
                    {
                        Status = Status.Success,
                        OrderId = result.OriginalTransactionId
                    });
                }
                catch (MobilePayException ex)
                {
                    _log.LogError("FAILED Could not refund order={order}. Error={ex}", completedOrder.OrderId, ex);
                    results.Add(new RefundResponse
                    {
                        Status = Status.Failed,
                        OrderId = completedOrder.OrderId
                    });
                }
            }

            await _outputWriter.WriteToFileAsync(results);
        }
    }
}
