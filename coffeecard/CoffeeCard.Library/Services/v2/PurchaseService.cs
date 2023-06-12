using System;
using System.Collections.Generic;
using System.Linq;
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
    public sealed class PurchaseService : IPurchaseService
    {
        private readonly CoffeeCardContext _context;
        private readonly IEmailService _emailService;
        private readonly IMobilePayPaymentsService _mobilePayPaymentsService;
        private readonly ITicketService _ticketService;
        private readonly IProductService _productService;

        public PurchaseService(CoffeeCardContext context, IMobilePayPaymentsService mobilePayPaymentsService,
            ITicketService ticketService, IEmailService emailService, IProductService productService)
        {
            _context = context;
            _mobilePayPaymentsService = mobilePayPaymentsService;
            _ticketService = ticketService;
            _emailService = emailService;
            _productService = productService;
        }

        public async Task<InitiatePurchaseResponse> InitiatePurchase(InitiatePurchaseRequest initiateRequest, User user)
        {
            var product = await _productService.GetProductAsync(initiateRequest.ProductId);
            CheckUserIsAllowedToPurchaseProduct(user, initiateRequest, product);

            Log.Information("Initiating purchase of ProductId {ProductId}, PaymentType {PurchaseType} by UserId {UserId}", initiateRequest.ProductId, initiateRequest.PaymentType, user.Id);

            var (purchase, paymentDetails) = await InitiatePaymentAsync(initiateRequest, product, user);

            await _context.Purchases.AddAsync(purchase);
            await _context.SaveChangesAsync();

            if (purchase.Status == PurchaseStatus.Completed)
            {
                Log.Information("Purchase {PurchaseId} has state Completed. Issues tickets", purchase.Id);
                await _ticketService.IssueTickets(purchase);
            }

            return new InitiatePurchaseResponse
            {
                Id = purchase.Id,
                DateCreated = purchase.DateCreated,
                ProductId = product.Id,
                ProductName = product.Name,
                TotalAmount = purchase.Price,
                PurchaseStatus = purchase.Status,
                PaymentDetails = paymentDetails
            };
        }

        /// <summary>
        /// Checks if a user is eligible to purchase a product
        /// Throws an ApiException if it does not pass the checks
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="initiateRequest">Purchase Request</param>
        /// <param name="product">Product</param>
        /// <exception cref="IllegalUserOperationException">User is not entitled to purchase product</exception>
        /// <exception cref="ArgumentException">PaymentType FreePurchase used for a non-free product</exception>
        private static void CheckUserIsAllowedToPurchaseProduct(User user, InitiatePurchaseRequest initiateRequest, Product product)
        {
            //Product does not belong to same userGroup as user
            if (!product.ProductUserGroup.Any(pug => pug.UserGroup == user.UserGroup))
            {
                Log.Warning(
                    "User {UserId} is not authorized to purchase Product Id: {ProductId} as user is not in Product User Group",
                    user.Id, product.Id);
                throw new IllegalUserOperationException($"User is not entitled to purchase product '{product.Name}'");
            }

            if (initiateRequest.PaymentType == PaymentType.FreePurchase && product.Price != 0)
            {
                Log.Warning(
                    "User tried to issue paid product to themselves, User {UserId}, Product {ProductId}",
                    user.Id, product.Id);
                throw new ArgumentException($"Product '{product.Name}' is not free");
            }
        }

        private async Task<(Purchase purchase, PaymentDetails paymentDetails)> InitiatePaymentAsync(InitiatePurchaseRequest purchaseRequest, Product product, User user)
        {
            var orderId = await GenerateUniqueOrderId();
            PaymentDetails paymentDetails;
            PurchaseStatus purchaseStatus;
            string transactionId;

            switch (purchaseRequest.PaymentType)
            {
                case PaymentType.MobilePay:
                    paymentDetails = await _mobilePayPaymentsService.InitiatePayment(new MobilePayPaymentRequest
                    {
                        Amount = product.Price,
                        OrderId = orderId,
                        Description = product.Name
                    });

                    purchaseStatus = PurchaseStatus.PendingPayment;
                    transactionId = ((MobilePayPaymentDetails)paymentDetails).PaymentId;

                    break;
                case PaymentType.FreePurchase:
                    paymentDetails = new FreePurchasePaymentDetails(orderId.ToString());
                    purchaseStatus = PurchaseStatus.Completed;
                    transactionId = Guid.Empty.ToString();

                    break;
                default:
                    Log.Error("Payment Type {PaymentType} is not handled in PurchaseService", purchaseRequest.PaymentType);
                    throw new ArgumentException($"Payment Type '{purchaseRequest.PaymentType}' is not handled");
            }

            var purchase = new Purchase
            {
                ProductName = product.Name,
                ProductId = product.Id,
                Price = product.Price,
                NumberOfTickets = product.NumberOfTickets,
                DateCreated = DateTime.UtcNow,
                OrderId = paymentDetails.OrderId,
                TransactionId = transactionId,
                PurchasedBy = user,
                Status = purchaseStatus
                // FIXME State management, PaymentType
            };

            return (purchase, paymentDetails);
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
                throw new EntityNotFoundException(
                    $"No purchase was found by Purchase Id: {purchaseId} and User Id: {user.Id}");
            }

            var paymentDetails = await _mobilePayPaymentsService.GetPayment(Guid.Parse(purchase.TransactionId));

            return new SinglePurchaseResponse
            {
                Id = purchase.Id,
                DateCreated = purchase.DateCreated,
                TotalAmount = purchase.Price,
                ProductId = purchase.ProductId,
                PurchaseStatus = purchase.Status,
                PaymentDetails = paymentDetails
            };
        }

        public async Task<IEnumerable<SimplePurchaseResponse>> GetPurchases(User user)
        {
            return await _context.Purchases
                .Where(p => p.PurchasedBy.Equals(user))
                .Select(p => new SimplePurchaseResponse
                {
                    Id = p.Id,
                    DateCreated = p.DateCreated,
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    NumberOfTickets = p.NumberOfTickets,
                    TotalAmount = p.Price,
                    PurchaseStatus = p.Status
                })
                .ToListAsync();
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
                throw new EntityNotFoundException(
                    $"No purchase was found by Transaction Id: {webhook.Data.Id} from webhook request");
            }

            if (purchase.Status == PurchaseStatus.Completed)
            {
                Log.Warning(
                    "Purchase from Webhook request is already completed. Purchase Id: {PurchaseId}, Transaction Id: {TransactionId}",
                    purchase.Id, webhook.Data.Id);
                return;
            }

            var eventTypeLowerCase = webhook.EventType.ToLower();
            switch (eventTypeLowerCase)
            {
                case "payment.reserved":
                    {
                        await CompletePurchase(purchase);
                        break;
                    }
                case "payment.cancelled_by_user":
                    {
                        await CancelPurchase(purchase);
                        break;
                    }
                case "payment.expired":
                    {
                        await CancelPurchase(purchase);
                        break;
                    }
                default:
                    Log.Error(
                        "Unknown EventType from Webhook request. Event Type: {EventType}, Purchase Id: {PurchaseId}, Transaction Id: {TransactionId}",
                        eventTypeLowerCase, purchase.Id, webhook.Data.Id);
                    throw new ArgumentException($"Event Type {eventTypeLowerCase} is not valid");
            }
        }

        private async Task CompletePurchase(Purchase purchase)
        {
            await _mobilePayPaymentsService.CapturePayment(Guid.Parse(purchase.TransactionId), purchase.Price);
            await _ticketService.IssueTickets(purchase);

            purchase.Status = PurchaseStatus.Completed;
            await _context.SaveChangesAsync();

            Log.Information("Completed purchase with Id {Id}, TransactionId {TransactionId}", purchase.Id, purchase.TransactionId);

            await _emailService.SendInvoiceAsyncV2(purchase, purchase.PurchasedBy);
        }

        private async Task CancelPurchase(Purchase purchase)
        {
            await _mobilePayPaymentsService.CancelPayment(Guid.Parse(purchase.TransactionId));
            purchase.Status = PurchaseStatus.Cancelled;
            await _context.SaveChangesAsync();

            Log.Information("Purchase has been cancelled Purchase Id {PurchaseId}, Transaction Id {TransactionId}",
                purchase.Id, purchase.TransactionId);
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

        public async Task<SimplePurchaseResponse> RedeemVoucher(string voucherCode, User user)
        {
            var voucher = await _context.Vouchers.Include(x => x.Product).FirstOrDefaultAsync(x => x.Code.Equals(voucherCode));
            if (voucher == null) throw new EntityNotFoundException($"Voucher '{voucherCode}' does not exist!");
            if (voucher.User != null) throw new ConflictException("Voucher has already been redeemed!");

            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == voucher.Product.Id);

            var purchase = new Purchase
            {
                DateCreated = DateTime.UtcNow,
                NumberOfTickets = voucher.Product.NumberOfTickets,
                OrderId = voucherCode,
                Price = 0,
                ProductId = voucher.Product.Id,
                ProductName = voucher.Product.Name,
                PurchasedBy = user
            };

            user.Purchases.Add(purchase);

            await _ticketService.IssueTickets(purchase);

            purchase.TransactionId = $"VOUCHER: {voucher.Id}";
            purchase.Status = PurchaseStatus.Completed;

            voucher.DateUsed = DateTime.UtcNow;
            voucher.User = user;

            _context.Vouchers.Attach(voucher);
            _context.Entry(voucher).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new SimplePurchaseResponse
            {
                Id = purchase.Id,
                DateCreated = purchase.DateCreated,
                ProductId = purchase.ProductId,
                ProductName = purchase.ProductName,
                NumberOfTickets = purchase.NumberOfTickets,
                TotalAmount = purchase.Price,
                PurchaseStatus = purchase.Status
            };
        }

        public void Dispose()
        {
            _context?.Dispose();
            _ticketService?.Dispose();
            _productService?.Dispose();
        }
    }
}