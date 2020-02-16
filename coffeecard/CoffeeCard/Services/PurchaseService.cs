using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.Helpers;
using CoffeeCard.Helpers.MobilePay;
using CoffeeCard.Helpers.MobilePay.ResponseMessage;
using CoffeeCard.Models;
using CoffeeCard.Models.DataTransferObjects.MobilePay;
using CoffeeCard.Models.DataTransferObjects.Purchase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace CoffeeCard.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly CoffeecardContext _context;
        private readonly IEmailService _emailService;
        private readonly IMobilePayService _mobilePayService;
        private readonly IConfiguration _configuration;
        private readonly IMapperService _mapper;

        public PurchaseService(CoffeecardContext context, IMobilePayService mobilePayService, IConfiguration configuration, IEmailService emailService, IMapperService mapper)
        {
            _context = context;
            _mobilePayService = mobilePayService;
            _configuration = configuration;
            _emailService = emailService;
            _mapper = mapper;
        }

        public bool Delete(int id)
        {
            var purchase = _context.Purchases.Find(id);
            if (purchase != null)
            {
                _context.Purchases.Remove(purchase);
                return _context.SaveChanges() > 0;
            }
            return false;
        }

        public Purchase Read(string orderId)
        {
            return _context.Purchases.FirstOrDefault(x => x.OrderId == orderId);
        }

        public IEnumerable<Purchase> Read(DateTime from, DateTime to)
        {
            return _context.Purchases.Where(x => x.DateCreated >= from && x.DateCreated <= to).ToList();
        }

        public IEnumerable<Purchase> Read(DateTime from)
        {
            return _context.Purchases.Where(x => x.DateCreated >= from).ToList();
        }

        public int Update(Purchase purchase)
        {
            _context.Purchases.Attach(purchase);
            return _context.SaveChanges();
        }

        public void Update()
        {
            var purchases = _context.Purchases.Where(x => x.PurchasedBy == null).ToList();
            _context.Purchases.RemoveRange(purchases);
            _context.SaveChanges();
        }

        public bool DeleteRange(List<Purchase> purchases)
        {
            _context.Purchases.RemoveRange(purchases);
            return _context.SaveChanges() > 0;
        }

        public Purchase GetPurchase(int id)
        {
            return _context.Purchases.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Purchase> GetPurchases(IEnumerable<Claim> claims)
        {
            var userId = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userId == null) throw new ApiException($"The token is invalid!", 401);
            var id = int.Parse(userId.Value);
            return _context.Purchases.Where(x => x.PurchasedBy.Id == id && x.Completed == true);
        }

        public Purchase RedeemVoucher(string voucherCode, IEnumerable<Claim> claims)
        {
            var userId = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userId == null) throw new ApiException($"The token is invalid!", 401);
            var id = int.Parse(userId.Value);

            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if (user == null) throw new ApiException($"The user could not be found");

            var voucher = _context.Vouchers.Include(x => x.Product).FirstOrDefault(x => x.Code.Equals(voucherCode));
            if (voucher == null) throw new ApiException($"Voucher '{voucherCode}' does not exist!");
            if (voucher.User != null) throw new ApiException($"Voucher has already been redeemed!");

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

            DeliverProductToUser(purchase, user, $"VOUCHER: {voucher.Id}");

            voucher.DateUsed = DateTime.UtcNow;
            voucher.User = user;

            _context.Vouchers.Attach(voucher);
            _context.Entry(voucher).State = EntityState.Modified;
            _context.SaveChanges();

            return purchase;
        }

        public Purchase DeliverProduct(CompletePurchaseDTO completeDto, IEnumerable<Claim> claims)
        {
            var userId = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userId == null) throw new ApiException($"The token is invalid!", 401);
            var id = int.Parse(userId.Value);

            var user = _context.Users.Include(x => x.Purchases).FirstOrDefault(x => x.Id == id);
            if (user == null) throw new ApiException($"The user could not be found");

            var purchase = user.Purchases.FirstOrDefault(x => x.OrderId == completeDto.OrderId);
            if (purchase == null) throw new ApiException($"Purchase could not be found");

            if (purchase.PurchasedBy.Id != user.Id) throw new ApiException($"You cannot complete a purchase that you did not initiate!", 401);

            return DeliverProductToUser(purchase, user, completeDto.TransactionId);
        }

        public Purchase DeliverProductToUser(Purchase purchase, User user, string transactionId)
        {
            Log.Information($"Delivering product ({purchase.ProductId}) to userId: {user.Id} with orderId: {purchase.OrderId}");
            var product = _context.Products.FirstOrDefault(x => x.Id == purchase.ProductId);
            if (product == null) throw new ApiException($"The product with id {purchase.ProductId} could not be found!");
            for (var i = 0; i < purchase.NumberOfTickets; i++)
            {
                var ticket = new Ticket() { ProductId = product.Id, Purchase = purchase };
                user.Tickets.Add(ticket);
            }

            purchase.TransactionId = transactionId;
            purchase.Completed = true;

            _context.Users.Attach(user);
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();

            Log.Information($"Delivery of product ({purchase.ProductId}) to userId: {user.Id} with orderId: {purchase.OrderId} succeeded!");
            return purchase;
        }

        public string InitiatePurchase(int productId, IEnumerable<Claim> claims)
        {
            Log.Information($"Initiating new purchase for productId: {productId}");
            var product = _context.Products.FirstOrDefault(x => x.Id == productId);
            if (product == null) throw new ApiException($"Product with id {productId} could not be found!", 400);

            var orderId = "";

            while (orderId == "")
            {
                var guid = Guid.NewGuid();
                if (!_context.Purchases.Any(x => x.OrderId.Equals(guid))) orderId = guid.ToString();
            }

            var purchase = new Purchase()
            {
                OrderId = orderId,
                Price = product.Price,
                ProductName = product.Name,
                ProductId = productId,
                NumberOfTickets = product.NumberOfTickets
            };

            var userId = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userId == null) throw new ApiException($"The token is invalid!", 401);
            var id = int.Parse(userId.Value);

            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if (user == null) throw new ApiException($"The user could not be found");

            user.Purchases.Add(purchase);

            _context.Attach(user);
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();

            Log.Information($"Purchase initiated with orderId: {purchase.OrderId} purchaseId: {purchase.Id} for user: {user.Id}");

            return orderId;
        }

        public async Task<Purchase> CompletePurchase(CompletePurchaseDTO dto, IEnumerable<Claim> claims)
        {
            Log.Information($"Trying to complete purchase with orderid: {dto.OrderId} and transactionId: {dto.TransactionId}");
            PaymentStatus paymentStatus;
            try
            {
                //TODO Figure out the purpose of this check, and probably fix it in regard to test environment
                if (!_configuration["MPMerchantID"].Equals("APPDK0000000000"))
                {
                    paymentStatus = await ValidateTransaction(dto);

                    switch (paymentStatus)
                    {
                        case PaymentStatus.Captured:
                            {
                                Log.Information($"Validating transaction with orderId: {dto.OrderId} and transactionId: {dto.TransactionId} succeeded!");
                                break;
                            }
                        case PaymentStatus.Reserved:
                            {
                                var captureResponse = await _mobilePayService.CapturePayment(dto.OrderId);
                                Log.Information($"Validating transaction with orderId: {dto.OrderId} and transactionId: {captureResponse.TransactionId} succeeded!");
                                break;
                            }
                        default:
                            {
                                Log.Warning($"Validating transaction at MobilePay failed with status: {paymentStatus}");
                                throw new ApiException($"The purchase could not be completed", 400);
                            }
                    }
                }
            }
            catch (MobilePayException e)
            {
                Log.Warning($"Complete purchase failed with error message: {e.Message} and status code: {e.GetHttpStatusCode()}");
                throw new ApiException("Failed to complete purchase using MobilePay", 400);
            }

            var purchase = DeliverProduct(dto, claims);
            SendInvoiceEmail(purchase);

            Log.Information($"Completed purchase with success! OrderId: {dto.OrderId} transactionId: {dto.TransactionId}");

            return purchase;
        }

        private void SendInvoiceEmail(Purchase purchase)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == purchase.PurchasedBy.Id);
            
            var purchaseDTO = _mapper.Map(purchase);
            var userDTO = _mapper.Map(user);

            _emailService.SendInvoice(userDTO, purchaseDTO);
        }

        private async Task<PaymentStatus> ValidateTransaction(CompletePurchaseDTO payment)
        {
            var purchase = _context.Purchases.FirstOrDefault(x => x.OrderId == payment.OrderId);
            if (purchase == null) throw new ApiException($"The purchase with orderId {payment.OrderId} does not exist", 400);
            if (purchase.Completed) throw new ApiException($"The given purchase has already been completed", 400);

            var response = await _mobilePayService.GetPaymentStatus(payment.OrderId);

            return response.LatestPaymentStatus;
        }

        /*  //TODO Reimplement
        public async Task CheckIncompletePurchases(User user)
        {
            var incompletePurchases = user.Purchases.Where(x => !x.Completed).ToList();
            Log.Information($"Checking {incompletePurchases.Count} purchases against mobilepay");
            foreach (var purchase in incompletePurchases)
            {
                var response = await ValidateTransactionWithoutId(purchase.OrderId);
                if (response != null)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var status = await response.Content.ReadAsAsync<MobilePayPaymentStatusDTO>();
                        if (status.LatestPaymentStatus.Equals("Captured") &&
                            status.OriginalAmount.Equals(purchase.Price))
                        {
                            var transactionId = status.TransactionId;
                            DeliverProductToUser(purchase, user, transactionId);
                        }
                        else if (status.LatestPaymentStatus.Equals("Rejected"))
                        {
                            _context.Purchases.Remove(purchase);
                        }
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        _context.Purchases.Remove(purchase);
                    }
                }
            }
            _context.SaveChanges();
        }

        public async Task<HttpResponseMessage> ValidateTransactionWithoutId(string orderId)
        {
            var response = await _mobilePayService.CapturePayment(orderId);
            return response;
        }*/

        public Purchase IssueProduct(IssueProductDTO issueProduct)
        {
            Log.Information($"Issuing product {issueProduct.ProductId} for user {issueProduct.UserId} with {issueProduct.IssuedBy} issuer id");
            //Check if the user exists
            var user = _context.Users.FirstOrDefault(x => x.Id == issueProduct.UserId);
            if (user == null) throw new ApiException("Invalid user id", 400);
            var product = _context.Products.FirstOrDefault(x => x.Id == issueProduct.ProductId);
            if (product == null) throw new ApiException("Invalid product ud", 400);

            Purchase purchase = new Purchase
            {
                Completed = true,
                NumberOfTickets = product.NumberOfTickets,
                OrderId = "Analog",
                Price = product.Price,
                ProductId = issueProduct.ProductId,
                ProductName = product.Name,
                DateCreated = DateTime.UtcNow,
                PurchasedBy = user,
                TransactionId = issueProduct.IssuedBy
            };

            for (var i = 0; i < product.NumberOfTickets; i++)
            {
                var ticket = new Ticket() { ProductId = product.Id, Purchase = purchase };
                user.Tickets.Add(ticket);
            }

            user.Purchases.Add(purchase);

            _context.Update(user);

            return purchase;
        }
        
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
