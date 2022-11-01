using System;
using System.Collections.Generic;
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
        private readonly IEmailService _emailService;
        private readonly IMobilePayPaymentsService _mobilePayPaymentsService;
        private readonly ITicketService _ticketService;

        public PurchaseService(CoffeeCardContext context, IMobilePayPaymentsService mobilePayPaymentsService,
            ITicketService ticketService, IEmailService emailService)
        {
            _context = context;
            _mobilePayPaymentsService = mobilePayPaymentsService;
            _ticketService = ticketService;
            _emailService = emailService;
        }

        public async Task<InitiatePurchaseResponse> InitiatePurchase(InitiatePurchaseRequest initiateRequest, User user)
        {
            var product = await _context.Products.Include(p => p.ProductUserGroup)
                .FirstOrDefaultAsync(p => p.Id == initiateRequest.ProductId);
            if (product == null)
            {
                Log.Error("No product was found by Product Id: {Id}", initiateRequest.ProductId);
                throw new EntityNotFoundException($"No purchase was found by Purchase Id: {initiateRequest.ProductId}");
                // Exception mapper 
            }

            var productUserGroup = product.ProductUserGroup.Select(pug => pug.UserGroup);
            var isUserInProductUserGroup = productUserGroup.Contains(user.UserGroup);

            if (!isUserInProductUserGroup)
            {
                Log.Information(
                    "User {UserId} is not authorized to purchase Product Id: {ProductId} as user is not in Product User Group",
                    user.Id, product.Id);
                throw new ApiException("Product is not available for user to purchase", StatusCodes.Status403Forbidden);
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
                PurchasedBy = user,
                Status = PurchaseStatus.PendingPayment
            };
            await _context.Purchases.AddAsync(purchase);
            await _context.SaveChangesAsync();

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

        public async Task<List<SimplePurchaseResponse>> GetPurchases(User user)
        {
            return await _context.Purchases
                .Where(p => p.PurchasedBy.Equals(user))
                .Select(p => new SimplePurchaseResponse
                {
                    Id = p.Id,
                    DateCreated = p.DateCreated,
                    ProductId = p.ProductId,
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

            if (purchase.Completed || purchase.Completed)
            {
                // FIXME Check purchase is not already completed. Should we throw an error? Conflict?
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

            purchase.Completed = true;
            purchase.Status = PurchaseStatus.Completed;
            await _context.SaveChangesAsync();

            await _emailService.SendInvoiceAsyncV2(purchase, purchase.PurchasedBy);
        }

        private async Task CancelPurchase(Purchase purchase)
        {
            await _mobilePayPaymentsService.CancelPayment(Guid.Parse(purchase.TransactionId));
            purchase.Completed = false;
            purchase.Status = PurchaseStatus.Cancelled;
            await _context.SaveChangesAsync();

            Log.Information("Purchase has been cancelled Purchase Id {PurchaseId}, Transaction Id {TransactionId}",
                purchase.Id, purchase.TransactionId);
        }

        public async Task<SinglePurchaseResponse> IssueFreeProduct(IssueProductDto issueProduct)
        {
            var (user, product) = await UserMayIssueFreeProduct(issueProduct.UserId, issueProduct.ProductId);

            Log.Information(
                "Issuing product {} for user {} with {} issuer id",
                issueProduct.ProductId, issueProduct.UserId, issueProduct.IssuedBy);
            var purchase = new Purchase
            {
                Completed = true,
                NumberOfTickets = product.NumberOfTickets,
                OrderId = (await GenerateUniqueOrderId()).ToString(),
                Price = product.Price,
                ProductId = issueProduct.ProductId,
                ProductName = product.Name,
                DateCreated = DateTime.UtcNow,
                PurchasedBy = user,
                TransactionId = issueProduct.IssuedBy,
                //TODO consider if this should include "status = PurchaseStatus.Completed 
            };

            for (var i = 0; i < product.NumberOfTickets; i++)
            {
                var ticket = new Ticket { ProductId = product.Id, Purchase = purchase };
                user.Tickets.Add (ticket);
            }

            user.Purchases.Add(purchase);

            _context.Update(user);

            await _context.SaveChangesAsync();

            return new SinglePurchaseResponse
            {
                Id = purchase.Id,
                DateCreated = purchase.DateCreated,
                TotalAmount = purchase.Price,
                ProductId = purchase.ProductId,
                PurchaseStatus = purchase.Status,
                PaymentDetails = new StaffPaymentDetails(purchase.OrderId)
            };
        }

        /// <summary>
        /// Checks if a user with the given Id may issue a free product of that Id
        /// Throws an ApiException if it does not pass the checks
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <returns>A tuple containing the User and Product of the given Ids</returns>
        /// <exception cref="ApiException"></exception>
        private async Task<Tuple<User, Product>> UserMayIssueFreeProduct(int userId, int productId)
        {
            var user = await _context.Users.FindAsync(userId);
            var product = await _context.Products
                .Include(p => p.ProductUserGroup)
                .FirstOrDefaultAsync(x => x.Id == productId);

            if (user == null)
            {
                Log.Error("No user was found by User Id: {Id}", userId);
                throw new ApiException($"No user was found by User Id: {userId}",
                    StatusCodes.Status404NotFound);
            }

            if (product == null)
            {
                Log.Error("No product was found by Product Id: {Id}", productId);
                throw new ApiException($"No product was found by Product Id: {productId}",
                    StatusCodes.Status404NotFound);
            }

            if (!product.ProductUserGroup.Any(pug => pug.UserGroup == user.UserGroup))
                //Product does not belong to same userGroup as user
            {
                Log.Warning(
                    "User tried to purchase product without being in correct user group, User {UserId}, Product {ProductId}",
                    user.Id, product.Id);
                throw new ApiException($"You do not have access to this product",
                    StatusCodes.Status403Forbidden);
            }
            
            if (product.Price != 0 ) //Product is not free
            {
                Log.Warning(
                    "User tried to issue paid product to themselves, User {UserId}, Product {ProductId}",
                    user.Id, product.Id);
                throw new ApiException($"Product is not free",
                    StatusCodes.Status403Forbidden);
            }

            return new Tuple<User, Product>(user, product);
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