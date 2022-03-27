using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.MobilePay.Service.v2;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Purchase = CoffeeCard.Models.Entities.Purchase;

namespace CoffeeCard.Library.Services.v2
{
    public class PurchaseService : IPurchaseService
    {
        private readonly CoffeeCardContext _context;
        private readonly IMobilePayPaymentsService _mobilePayPaymentsService;
        private readonly ITicketService _ticketService;

        public PurchaseService(CoffeeCardContext context, IMobilePayPaymentsService mobilePayPaymentsService, ITicketService ticketService)
        {
            _context = context;
            _mobilePayPaymentsService = mobilePayPaymentsService;
            _ticketService = ticketService;
        }

        public async Task<InitiatePurchaseResponse> InitiatePurchase(InitiatePurchaseRequest initiateRequest, User user)
        {
            var product = await _context.Products.Include(p => p.ProductUserGroup).FirstOrDefaultAsync(p => p.Id == initiateRequest.ProductId);
            if (product == null)
            {
                Log.Error("No product was found by Product Id: {Id}", initiateRequest.ProductId);
                // throw EntityNotFoundException mapping to a ProblemDetails object
            }
            ;
            var userGroups = product.ProductUserGroup.Select(x => x.UserGroup);
            var canUserPurchase = userGroups.Contains(user.UserGroup);

            if (!canUserPurchase)
            {
                Log.Information("User {userId} is not authorized to purchase product {productId}", user.Id, product.Id);
                throw new ApiException("User is unable to purchase selected product", StatusCodes.Status403Forbidden);
            }

            var paymentDetails = await _mobilePayPaymentsService.InitiatePayment(new MobilePayPaymentRequest
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
                TransactionId = paymentDetails.PaymentId,
                PurchasedBy = user
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

        public async Task<SinglePurchaseResponse> GetPurchase(int purchaseId, User user)
        {
            var purchase = await _context.Purchases
                .Include(p => p.PurchasedBy)
                .Where(p => p.Id == purchaseId 
                            && p.PurchasedBy.Equals(user))
                .FirstOrDefaultAsync();
            if (purchase == null)
            {
                Log.Error("No purchase was found by Purchase Id: {Id} and User Id: {UId}", purchaseId, user.Id);
                throw new EntityNotFoundException($"No purchase was found by Purchase Id: {purchaseId}");
            }
            
            var paymentDetails = await _mobilePayPaymentsService.GetPayment(Guid.Parse(purchase.TransactionId));

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
            var purchase = await _context.Purchases
                .Include(p => p.PurchasedBy)
                .Where(p => p.TransactionId.Equals(webhook.Data.Id))
                .FirstOrDefaultAsync();
            if (purchase == null)
            {
                Log.Error("No purchase was found by TransactionId: {Id} from Webhook request", webhook.Data.Id);
                throw new EntityNotFoundException($"No purchase was found by TransactionId: {webhook.Data.Id} from webhook request");
            }
            
            // FIXME Check purchase is not already completed

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
            await _mobilePayPaymentsService.CapturePayment(Guid.Parse(purchase.TransactionId), purchase.Price);
            await _ticketService.IssueTickets(purchase);
            // FIXME Send email
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