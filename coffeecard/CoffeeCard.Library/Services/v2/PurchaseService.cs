using System;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Library.Persistence;
using CoffeeCard.MobilePay.Service.v2;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Purchase = CoffeeCard.Models.Entities.Purchase;

namespace CoffeeCard.Library.Services.v2
{
    public class PurchaseService : IPurchaseService
    {
        private readonly CoffeeCardContext _context;
        private readonly IMobilePayService _mobilePayService;
        private readonly ITicketService _ticketService;

        public PurchaseService(CoffeeCardContext context, IMobilePayService mobilePayService, ITicketService ticketService)
        {
            _context = context;
            _mobilePayService = mobilePayService;
            _ticketService = ticketService;
        }

        public async Task<InitiatePurchaseResponse> InitiatePurchase(InitiatePurchaseRequest initiateRequest)
        {
            var product = await _context.Products.FindAsync(initiateRequest.ProductId);
            if (product == null)
            {
                Log.Error("No product was found by Product Id: {Id}", initiateRequest.ProductId);
                // throw EntityNotFoundException mapping to a ProblemDetails object
            }

            var paymentDetails = await _mobilePayService.InitiatePayment(new MobilePayPaymentRequest
            {
                Amount = product.Price,
                OrderId = await GenerateUniqueOrderId(),
                Description = product.Name
            });

            // FIXME State management, PaymentType
            var purchase = new Purchase
            {
                ProductName = product.Name,
                ProductId = product.Id,
                Price = product.Price,
                NumberOfTickets = product.NumberOfTickets,
                DateCreated = DateTime.UtcNow,
                Completed = false,
                OrderId = paymentDetails.OrderId,
                TransactionId = paymentDetails.PaymentId
            };
            await _context.Purchases.AddAsync(purchase);
            await _context.SaveChangesAsync();

            return new InitiatePurchaseResponse
            {
                Id = purchase.Id,
                DateCreated = purchase.DateCreated,
                ProductId = product.Id,
                TotalAmount = purchase.Price,
                PurchaseStatus = PurchaseStatus.PendingPayment,
                PaymentDetails = paymentDetails
            };
        }

        public async Task<SinglePurchaseResponse> GetPurchase(int purchaseId)
        {
            var purchase = await _context.Purchases.FindAsync(purchaseId);
            if (purchase == null)
            {
                Log.Error("No purchase was found by Purchase Id: {Id}", purchaseId);
                // throw exception
            }
            
            // FIXME. Verify ownership. A user can only get a purchase which the user owns

            var paymentDetails = await _mobilePayService.GetPayment(Guid.Parse(purchase.TransactionId));

            return new SinglePurchaseResponse
            {
                Id = purchase.Id,
                DateCreated = purchase.DateCreated,
                TotalAmount = purchase.Price,
                ProductId = purchase.ProductId,
                PurchaseStatus = PurchaseStatus.Completed, // FIXME Handle state
                PaymentDetails = paymentDetails
            };
        }

        public async Task HandleMobilePayPaymentUpdate(MobilePayWebhook webhook)
        {
            var purchase = await _context.Purchases.Where(p => p.TransactionId.Equals(webhook.Data.Id)).FirstAsync();
            if (purchase == null)
            {
                Log.Error("No purchase was found by TransactionId: {Id}", webhook.Data.Id);
                // throw some exception
            }

            switch (webhook.EventType.ToLower())
            {
                case "payment.reserved":
                {
                    await CompletePurchase(purchase);
                    purchase.Completed = true;
                    break;
                }
                case "payment.cancelled_by_user":
                {
                    purchase.Completed = false;
                    break;
                }
                case "payment.expired":
                {
                    purchase.Completed = false;
                    break;
                }
                default:
                    // log, throw exception
                break;
            }

            await _context.SaveChangesAsync();
        }

        private async Task CompletePurchase(Purchase purchase)
        {
            await _ticketService.IssueTickets(purchase);
            // Send email
        }

        private async Task<Guid> GenerateUniqueOrderId()
        {
            while (true)
            {
                var newOrderId = Guid.NewGuid();

                var orderIdAlreadyExists =
                    await _context.Purchases.Where(p => p.OrderId.Equals(newOrderId.ToString())).AnyAsync();
                if (orderIdAlreadyExists) continue;

                return newOrderId;
            }
        }
    }
}