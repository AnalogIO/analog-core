using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.Exception;
using CoffeeCard.MobilePay.RefundConsoleApp.IO;
using CoffeeCard.MobilePay.RefundConsoleApp.Model;
using CoffeeCard.MobilePay.Service;
using CoffeeCard.MobilePay.Service.v1;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.MobilePay.RefundConsoleApp.Handler
{
    public class RefundHandler
    {
        private readonly ILogger<RefundHandler> _log;
        private readonly IMobilePayService _mobilePayService;
        private readonly IOutputWriter<IList<RefundResponse>> _outputWriter;

        public RefundHandler(
            ILogger<RefundHandler> log,
            IOutputWriter<IList<RefundResponse>> outputWriter,
            IMobilePayService mobilePayService
        )
        {
            _log = log;
            _mobilePayService = mobilePayService;
            _outputWriter = outputWriter;
        }

        public async Task RefundPayments(IEnumerable<CompletedOrder> completedOrders)
        {
            var results = new List<RefundResponse>();
            foreach (var completedOrder in completedOrders)
                try
                {
                    var result = await _mobilePayService.RefundPayment(completedOrder.OrderId);
                    _log.LogInformation("SUCCESS Refunded order={result}", result);

                    results.Add(
                        new RefundResponse(completedOrder.OrderId, Status.Success)
                        {
                            OriginalTransactionId = result.OriginalTransactionId,
                            RefundTransactionId = result.TransactionId,
                            Remainder = result.Remainder,
                        }
                    );
                }
                catch (MobilePayException ex)
                {
                    _log.LogError(
                        "FAILED Could not refund order={order}. Error={ex}",
                        completedOrder.OrderId,
                        ex
                    );
                    results.Add(new RefundResponse(completedOrder.OrderId, Status.Failed));
                }

            await _outputWriter.WriteToFileAsync(results);
        }
    }
}
