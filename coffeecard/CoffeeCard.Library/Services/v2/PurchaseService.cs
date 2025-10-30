using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.MobilePay.Generated.Api.ePaymentApi;
using CoffeeCard.MobilePay.Service.v2;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using CoffeeCard.Models.DataTransferObjects.v2.Products;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Purchase = CoffeeCard.Models.Entities.Purchase;

namespace CoffeeCard.Library.Services.v2
{
    public sealed class PurchaseService : IPurchaseService
    {
        private readonly CoffeeCardContext _context;
        private readonly CoffeeCard.Library.Services.IEmailService _emailService;
        private readonly IMobilePayPaymentsService _mobilePayPaymentsService;
        private readonly ITicketService _ticketService;
        private readonly IProductService _productService;
        private readonly ILogger<PurchaseService> _logger;

        public PurchaseService(
            CoffeeCardContext context,
            IMobilePayPaymentsService mobilePayPaymentsService,
            ITicketService ticketService,
            CoffeeCard.Library.Services.IEmailService emailService,
            IProductService productService,
            ILogger<PurchaseService> logger
        )
        {
            _context = context;
            _mobilePayPaymentsService = mobilePayPaymentsService;
            _ticketService = ticketService;
            _emailService = emailService;
            _productService = productService;
            _logger = logger;
        }

        public async Task<InitiatePurchaseResponse> InitiatePurchase(
            InitiatePurchaseRequest initiateRequest,
            User user
        )
        {
            var product = await _productService.GetProductAsync(initiateRequest.ProductId);
            CheckUserIsAllowedToPurchaseProduct(user, initiateRequest, product);

            _logger.LogInformation(
                "Initiating purchase of ProductId {ProductId}, PaymentType {PurchaseType} by UserId {UserId}",
                initiateRequest.ProductId,
                initiateRequest.PaymentType,
                user.Id
            );

            var (purchase, paymentDetails) = await InitiatePaymentAsync(
                initiateRequest,
                product,
                user
            );

            await _context.Purchases.AddAsync(purchase);
            await _context.SaveChangesAsync();

            if (purchase.Status == PurchaseStatus.Completed)
            {
                _logger.LogInformation(
                    "Purchase {PurchaseId} has state Completed. Issues tickets",
                    purchase.Id
                );
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
                PaymentDetails = paymentDetails,
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
        /// <exception cref="BadRequestException">PaymentType FreePurchase used for a non-free product</exception>
        private void CheckUserIsAllowedToPurchaseProduct(
            User user,
            InitiatePurchaseRequest initiateRequest,
            ProductResponse product
        )
        {
            //Product does not belong to same userGroup as user
            if (!product.AllowedUserGroups.Any(ug => ug == user.UserGroup))
            {
                _logger.LogWarning(
                    "User {UserId} is not authorized to purchase Product Id: {ProductId} as user is not in Product User Group",
                    user.Id,
                    product.Id
                );
                throw new IllegalUserOperationException(
                    $"User is not entitled to purchase product '{product.Name}'"
                );
            }

            if (initiateRequest.PaymentType == PaymentType.FreePurchase && product.Price != 0)
            {
                _logger.LogWarning(
                    "User tried to issue paid product to themselves, User {UserId}, Product {ProductId}",
                    user.Id,
                    product.Id
                );
                throw new BadRequestException($"Product '{product.Name}' is not free");
            }
        }

        private async Task<(Purchase purchase, PaymentDetails paymentDetails)> InitiatePaymentAsync(
            InitiatePurchaseRequest purchaseRequest,
            ProductResponse product,
            User user
        )
        {
            var orderId = await GenerateUniqueOrderId();
            PaymentDetails paymentDetails;
            PurchaseStatus purchaseStatus;
            string? transactionId;

            switch (purchaseRequest.PaymentType)
            {
                case PaymentType.MobilePay:
                    paymentDetails = await _mobilePayPaymentsService.InitiatePayment(
                        new MobilePayPaymentRequest
                        {
                            Amount = product.Price,
                            OrderId = orderId,
                            Description = product.Name,
                        }
                    );

                    purchaseStatus = PurchaseStatus.PendingPayment;
                    transactionId = ((MobilePayPaymentDetails)paymentDetails).PaymentId;

                    break;
                case PaymentType.FreePurchase:
                    paymentDetails = new FreePurchasePaymentDetails(orderId.ToString());
                    purchaseStatus = PurchaseStatus.Completed;
                    transactionId = null;

                    break;
                default:
                    _logger.LogError(
                        "Payment Type {PaymentType} is not handled in PurchaseService",
                        purchaseRequest.PaymentType
                    );
                    throw new BadRequestException(
                        $"Payment Type '{purchaseRequest.PaymentType}' is not handled"
                    );
            }

            var purchase = new Purchase
            {
                ProductName = product.Name,
                ProductId = product.Id,
                Price = product.Price,
                NumberOfTickets = product.NumberOfTickets,
                DateCreated = DateTime.UtcNow,
                OrderId = orderId.ToString(),
                ExternalTransactionId = transactionId,
                PurchasedBy = user,
                Status = purchaseStatus,
                Type = purchaseRequest.PaymentType.ToPurchaseType(),
            };

            return (purchase, paymentDetails);
        }

        public async Task<SinglePurchaseResponse> GetPurchase(int purchaseId, User user)
        {
            var purchase = await _context
                .Purchases.Include(p => p.PurchasedBy)
                .Where(p => p.Id == purchaseId && p.PurchasedBy.Equals(user))
                .FirstOrDefaultAsync();
            if (purchase == null)
            {
                _logger.LogError(
                    "No purchase was found by Purchase Id: {Id} and User Id: {UId}",
                    purchaseId,
                    user.Id
                );
                throw new EntityNotFoundException(
                    $"No purchase was found by Purchase Id: {purchaseId} and User Id: {user.Id}"
                );
            }

            var paymentDetails = await _mobilePayPaymentsService.GetPayment(
                Guid.Parse(purchase.ExternalTransactionId)
            );

            return new SinglePurchaseResponse
            {
                Id = purchase.Id,
                DateCreated = purchase.DateCreated,
                TotalAmount = purchase.Price,
                ProductId = purchase.ProductId,
                PurchaseStatus = purchase.Status,
                PaymentDetails = paymentDetails,
            };
        }

        public async Task<IEnumerable<SimplePurchaseResponse>> GetPurchases(int userId)
        {
            var user = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                _logger.LogError("No user was found by User Id: {Id}", userId);
                throw new EntityNotFoundException($"No user was found by User Id: {userId}");
            }

            return await GetPurchases(user);
        }

        public async Task<IEnumerable<SimplePurchaseResponse>> GetPurchases(User user)
        {
            return await _context
                .Purchases.Where(p => p.PurchasedBy.Equals(user))
                .Select(p => new SimplePurchaseResponse
                {
                    Id = p.Id,
                    DateCreated = p.DateCreated,
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    NumberOfTickets = p.NumberOfTickets,
                    TotalAmount = p.Price,
                    PurchaseStatus = p.Status,
                })
                .ToListAsync();
        }

        public async Task HandleMobilePayPaymentUpdate(WebhookEvent webhook)
        {
            var purchase = await _context
                .Purchases.Include(p => p.PurchasedBy)
                .Where(p => p.ExternalTransactionId.Equals(webhook.Reference))
                .FirstOrDefaultAsync();
            if (purchase == null)
            {
                _logger.LogError(
                    "No purchase was found by TransactionId: {Id} from Webhook request",
                    webhook.Reference
                );
                throw new EntityNotFoundException(
                    $"No purchase was found by Transaction Id: {webhook.Reference} from webhook request"
                );
            }

            if (purchase.Status == PurchaseStatus.Completed)
            {
                _logger.LogWarning(
                    "Purchase from Webhook request is already completed. Purchase Id: {PurchaseId}, Transaction Id: {TransactionId}",
                    purchase.Id,
                    webhook.Reference
                );
                return;
            }

            var eventTypeLowerCase = webhook.Name;
            switch (eventTypeLowerCase)
            {
                case PaymentEventName.AUTHORIZED:
                {
                    await CompletePurchase(purchase);
                    break;
                }
                case PaymentEventName.CANCELLED:
                {
                    await CancelPurchase(purchase);
                    break;
                }
                case PaymentEventName.ABORTED:
                {
                    await AbortPurchase(purchase);
                    break;
                }
                case PaymentEventName.EXPIRED:
                {
                    await ExpirePurchase(purchase);
                    break;
                }
                default:
                    _logger.LogError(
                        "Unknown EventType from Webhook request. Event Type: {EventType}, Purchase Id: {PurchaseId}, Transaction Id: {TransactionId}",
                        eventTypeLowerCase,
                        purchase.Id,
                        webhook.Reference
                    );
                    throw new BadRequestException($"Event Type {eventTypeLowerCase} is not valid");
            }
        }

        private async Task CompletePurchase(Purchase purchase)
        {
            await _mobilePayPaymentsService.CapturePayment(
                Guid.Parse(purchase.ExternalTransactionId),
                purchase.Price
            );
            await _ticketService.IssueTickets(purchase);

            purchase.Status = PurchaseStatus.Completed;
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Completed purchase with Id {Id}, TransactionId {TransactionId}",
                purchase.Id,
                purchase.ExternalTransactionId
            );

            await _emailService.SendInvoiceAsyncV2(purchase, purchase.PurchasedBy);
        }

        private async Task CancelPurchase(Purchase purchase)
        {
            await _mobilePayPaymentsService.CancelPayment(
                Guid.Parse(purchase.ExternalTransactionId)
            );
            purchase.Status = PurchaseStatus.Cancelled;
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Purchase has been cancelled: Purchase Id {PurchaseId}, Transaction Id {TransactionId}",
                purchase.Id,
                purchase.ExternalTransactionId
            );
        }

        private async Task AbortPurchase(Purchase purchase)
        {
            purchase.Status = PurchaseStatus.Cancelled;
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Purchase was aborted by user: Purchase Id {PurchaseId}, Transaction Id {TransactionId}",
                purchase.Id,
                purchase.ExternalTransactionId
            );
        }

        private async Task ExpirePurchase(Purchase purchase)
        {
            purchase.Status = PurchaseStatus.Expired;
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "User failed to complete purchase within expected: Purchase Id {PurchaseId}, Transaction Id {TransactionId}",
                purchase.Id,
                purchase.ExternalTransactionId
            );
        }

        private async Task<Guid> GenerateUniqueOrderId()
        {
            while (true)
            {
                var newOrderId = Guid.NewGuid();

                var orderIdAlreadyExists = await _context
                    .Purchases.Where(p => p.OrderId.Equals(newOrderId.ToString()))
                    .AnyAsync();
                if (orderIdAlreadyExists)
                    continue;

                return newOrderId;
            }
        }

        public async Task<SimplePurchaseResponse> RedeemVoucher(string voucherCode, User user)
        {
            var voucher = await _context
                .Vouchers.Where(v => v.Code.Equals(voucherCode))
                .Include(v => v.Product)
                .FirstOrDefaultAsync();
            if (voucher == null)
                throw new EntityNotFoundException($"Voucher '{voucherCode}' does not exist");
            if (voucher.UserId != null)
                throw new ConflictException($"Voucher '{voucherCode}' has already been redeemed");

            var purchase = new Purchase
            {
                DateCreated = DateTime.UtcNow,
                NumberOfTickets = voucher.Product.NumberOfTickets,
                OrderId = (await GenerateUniqueOrderId()).ToString(),
                Price = 0,
                ProductId = voucher.Product.Id,
                ProductName = voucher.Product.Name,
                PurchasedBy = user,
                Status = PurchaseStatus.Completed,
                Type = PurchaseType.Voucher,
            };

            user.Purchases.Add(purchase);

            await _ticketService.IssueTickets(purchase);

            purchase.ExternalTransactionId = null;
            purchase.Status = PurchaseStatus.Completed;

            voucher.DateUsed = DateTime.UtcNow;
            voucher.User = user;
            voucher.Purchase = purchase;

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
                PurchaseStatus = purchase.Status,
            };
        }

        public async Task<SimplePurchaseResponse> RefundPurchase(int paymentId)
        {
            var purchase = await _context
                .Purchases.Where(p => p.Id == paymentId)
                .Include(p => p.PurchasedBy)
                .Include(p => p.Tickets)
                .FirstOrDefaultAsync();

            // Does the purchase exist?
            if (purchase == null)
            {
                _logger.LogError("No purchase was found by Purchase Id: {Id}", paymentId);
                throw new EntityNotFoundException(
                    $"No purchase was found by Purchase Id: {paymentId}"
                );
            }

            // Is the purchase in a state where it can be refunded?
            if (purchase.Status != PurchaseStatus.Completed)
            {
                _logger.LogError(
                    "Purchase {PurchaseId} is not in state Completed. Cannot refund",
                    purchase.Id
                );
                throw new IllegalUserOperationException(
                    $"Purchase {purchase.Id} is not in state Completed. Cannot refund"
                );
            }

            // Are all of the tickets unused (i.e. refundable)?
            if (purchase.Tickets.Any(t => !t.IsConsumable))
            {
                _logger.LogError(
                    "Purchase {PurchaseId} has tickets that are not unused. Cannot refund",
                    purchase.Id
                );
                throw new IllegalUserOperationException(
                    $"Purchase {purchase.Id} has tickets that are not unused. Cannot refund"
                );
            }

            // Refund the MobilePay payment
            // (MobilePay expects refund amount in øre; we store the price in kroner)
            var amountToRefund = purchase.Price * 100;
            var refundSuccess = await _mobilePayPaymentsService.RefundPayment(
                purchase,
                amountToRefund
            );
            if (!refundSuccess)
            {
                _logger.LogError("Refund of Purchase {PurchaseId} failed", purchase.Id);
                throw new InvalidOperationException($"Refund of Purchase {purchase.Id} failed");
            }

            // Set the tickets' status to refunded
            foreach (var ticket in purchase.Tickets)
            {
                ticket.Status = TicketStatus.Refunded;
            }

            // Set the purchase status to refunded
            purchase.Status = PurchaseStatus.Refunded;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Refunded Purchase {PurchaseId}", purchase.Id);

            return new SimplePurchaseResponse
            {
                Id = purchase.Id,
                DateCreated = purchase.DateCreated,
                ProductId = purchase.ProductId,
                ProductName = purchase.ProductName,
                NumberOfTickets = purchase.NumberOfTickets,
                TotalAmount = purchase.Price,
                PurchaseStatus = purchase.Status,
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
