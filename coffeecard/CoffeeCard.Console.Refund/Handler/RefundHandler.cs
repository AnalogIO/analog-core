using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CoffeeCard.Console.Refund.IO;
using CoffeeCard.Console.Refund.Model;
using CoffeeCard.Helpers.MobilePay;
using CoffeeCard.Helpers.MobilePay.ResponseMessage;
using CoffeeCard.Services;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CoffeeCard.Console.Refund.Handler
{
    public class RefundHandler
    {
        private ILogger<RefundHandler> _log;
        private IMobilePayService _mobilePayService;
        private IOutputWriter<IList<RefundResponse>> _outputWriter;

        public RefundHandler(ILogger<RefundHandler> log, IMobilePayService mobilePayService, IOutputWriter<IList<RefundResponse>> outputWriter)
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
