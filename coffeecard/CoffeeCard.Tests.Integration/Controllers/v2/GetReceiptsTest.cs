using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using CoffeeCard.Models.Entities;
using CoffeeCard.Tests.ApiClient.Generated;
using CoffeeCard.Tests.ApiClient.v2.Generated;
using CoffeeCard.Tests.Common.Builders;
using CoffeeCard.Tests.Integration.WebApplication;
using CoffeeCard.WebApi;
using Xunit;
using Xunit;
using EntityPurchaseStatus = CoffeeCard.Models.DataTransferObjects.v2.Purchase.PurchaseStatus;
using PurchaseStatus = CoffeeCard.Models.DataTransferObjects.v2.Purchase.PurchaseStatus;
using UserGroup = CoffeeCard.Models.Entities.UserGroup;

namespace CoffeeCard.Tests.Integration.Controllers.v2
{
    public class GetReceiptsTest(CustomWebApplicationFactory<Startup> factory)
        : BaseIntegrationTest(factory)
    {
        [Fact]
        public async Task GetReceipts_returns_401_when_not_authenticated()
        {
            RemoveRequestHeaders();

            var exception = await Assert.ThrowsAsync<ApiException>(async () =>
                await CoffeeCardClientV2.Receipt_GetReceiptsAsync(ReceiptType.All, 20, null)
            );

            Assert.Equal(401, exception.StatusCode);
        }

        [Fact]
        public async Task GetReceipts_returns_empty_list_when_user_has_no_activity()
        {
            await GetAuthenticatedUserAsync();

            var response = await CoffeeCardClientV2.Receipt_GetReceiptsAsync(
                ReceiptType.All,
                20,
                null
            );

            Assert.NotNull(response);
            Assert.Empty(response.Receipts);
        }

        [Fact]
        public async Task GetReceipts_returns_purchase_receipt_for_completed_purchase()
        {
            var user = await GetAuthenticatedUserAsync();

            var purchase = PurchaseBuilder
                .Simple()
                .WithPurchasedBy(user)
                .WithPrice(100)
                .WithNumberOfTickets(10)
                .WithStatus(PurchaseStatus.Completed)
                .WithType(PurchaseType.MobilePayV2)
                .WithOrderId(Guid.NewGuid().ToString())
                .WithDateCreated(new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc))
                .WithTickets(new List<Ticket>())
                .WithVoucher(f => null)
                .Build();

            await Context.Purchases.AddAsync(purchase);
            await Context.SaveChangesAsync();

            var response = await CoffeeCardClientV2.Receipt_GetReceiptsAsync(
                ReceiptType.Purchase,
                20,
                null
            );

            Assert.Single(response.Receipts);
            var receipt = Assert.IsType<PurchaseReceipt>(response.Receipts.First());
            Assert.Equal(purchase.ProductName, receipt.ProductName);
            Assert.Equal(purchase.Price, receipt.Price);
            Assert.Equal(purchase.NumberOfTickets, receipt.Quantity);
            Assert.Equal(Guid.Parse(purchase.OrderId), receipt.OrderId);
        }

        [Fact]
        public async Task GetReceipts_returns_used_ticket_receipt_for_used_ticket()
        {
            var user = await GetAuthenticatedUserAsync();
            var swipeDate = new Faker().Date.Past().ToUniversalTime();

            var purchase = PurchaseBuilder
                .Simple()
                .WithPurchasedBy(user)
                .WithStatus(PurchaseStatus.Completed)
                .WithType(PurchaseType.Free)
                .WithTickets(
                    TicketBuilder
                        .Simple()
                        .WithStatus(TicketStatus.Used)
                        .WithDateUsed(swipeDate)
                        .WithOwner(user)
                        .Build(1)
                )
                .Build();

            await Context.Purchases.AddAsync(purchase);
            await Context.SaveChangesAsync();

            var response = await CoffeeCardClientV2.Receipt_GetReceiptsAsync(
                ReceiptType.UsedTicket,
                20,
                null
            );

            Assert.Single(response.Receipts);
            var receipt = Assert.IsType<UsedTicketReceipt>(response.Receipts.First());
            Assert.Equal(purchase.ProductName, receipt.ProductName);

            // We compare the string representations of the dates to avoid issues with precision loss (nanoseconds) when converting between DateTime and DateTimeOffset
            Assert.Equal(swipeDate.ToString("F"), receipt.SwipeDate.UtcDateTime.ToString("F"));
        }

        [Fact]
        public async Task GetReceipts_returns_voucher_receipt_for_voucher_purchase()
        {
            var user = await GetAuthenticatedUserAsync();
            var redeemDate = new DateTime(2026, 1, 3, 9, 0, 0, DateTimeKind.Utc);

            var product = ProductBuilder.Simple().Build();
            await Context.Products.AddAsync(product);

            var voucher = VoucherBuilder
                .Simple()
                .WithCode("TESTVOUCHER")
                .WithProduct(product)
                .WithUser(user)
                .WithDateCreated(redeemDate)
                .WithDateUsed(redeemDate)
                .WithDescription(f => null)
                .WithRequester(f => null)
                .Build();

            var purchase = PurchaseBuilder
                .Simple()
                .WithPurchasedBy(user)
                .WithPrice(0)
                .WithNumberOfTickets(5)
                .WithStatus(PurchaseStatus.Completed)
                .WithType(PurchaseType.Voucher)
                .WithOrderId(Guid.NewGuid().ToString())
                .WithDateCreated(redeemDate)
                .WithTickets(new List<Ticket>())
                .WithVoucher(voucher)
                .Build();

            await Context.Purchases.AddAsync(purchase);
            await Context.SaveChangesAsync();

            var response = await CoffeeCardClientV2.Receipt_GetReceiptsAsync(
                ReceiptType.Voucher,
                20,
                null
            );

            Assert.Single(response.Receipts);
            var receipt = Assert.IsType<VoucherReceipt>(response.Receipts.First());
            Assert.Equal(purchase.ProductName, receipt.ProductName);
            Assert.Equal("TESTVOUCHER", receipt.Code);
            Assert.Equal(purchase.NumberOfTickets, receipt.Quantity);
            Assert.Equal(redeemDate, receipt.RedeemDate.UtcDateTime);
        }

        [Fact]
        public async Task GetReceipts_only_returns_receipts_for_authenticated_user()
        {
            var user = await GetAuthenticatedUserAsync();

            var otherPurchase = PurchaseBuilder.Simple().Build();

            var myPurchase = PurchaseBuilder
                .Simple()
                .WithPurchasedBy(user)
                .WithType(PurchaseType.MobilePayV2)
                .Build();

            await Context.Purchases.AddRangeAsync(otherPurchase, myPurchase);
            await Context.SaveChangesAsync();

            var response = await CoffeeCardClientV2.Receipt_GetReceiptsAsync(
                ReceiptType.Purchase,
                20,
                null
            );

            Assert.Single(response.Receipts);
            var receipt = Assert.IsType<PurchaseReceipt>(response.Receipts.First());
            Assert.Equal(myPurchase.ProductName, receipt.ProductName);
        }

        [Fact]
        public async Task GetReceipts_respects_batch_size()
        {
            var user = await GetAuthenticatedUserAsync();

            var purchases = PurchaseBuilder
                .Simple()
                .WithPurchasedBy(user)
                .WithStatus(PurchaseStatus.Completed)
                .WithType(PurchaseType.MobilePayV2)
                .Build(5);

            await Context.Purchases.AddRangeAsync(purchases);

            await Context.SaveChangesAsync();

            var response = await CoffeeCardClientV2.Receipt_GetReceiptsAsync(
                ReceiptType.Purchase,
                3,
                null
            );

            Assert.Equal(3, response.Receipts.Count);
        }

        [Fact]
        public async Task GetReceipts_with_continuation_token_returns_next_batch()
        {
            var user = await GetAuthenticatedUserAsync();

            var purchases = PurchaseBuilder
                .Simple()
                .WithPurchasedBy(user)
                .WithStatus(PurchaseStatus.Completed)
                .WithType(PurchaseType.MobilePayV2)
                .Build(4);

            await Context.Purchases.AddRangeAsync(purchases);

            await Context.SaveChangesAsync();

            // Get the first batch of 2
            var firstPage = await CoffeeCardClientV2.Receipt_GetReceiptsAsync(
                ReceiptType.Purchase,
                2,
                null
            );
            Assert.Equal(2, firstPage.Receipts.Count);
            Assert.NotNull(firstPage.ContinueationToken);

            // Use the continuation token to get the next batch
            var secondPage = await CoffeeCardClientV2.Receipt_GetReceiptsAsync(
                ReceiptType.Purchase,
                2,
                firstPage.ContinueationToken
            );
            Assert.Equal(2, secondPage.Receipts.Count);

            // The two pages should not overlap
            var firstPageDates = firstPage
                .Receipts.Cast<PurchaseReceipt>()
                .Select(r => r.OrderDate)
                .ToHashSet();
            var secondPageDates = secondPage
                .Receipts.Cast<PurchaseReceipt>()
                .Select(r => r.OrderDate)
                .ToHashSet();
            Assert.Empty(firstPageDates.Intersect(secondPageDates));
        }

        [Fact]
        public async Task GetReceipts_with_All_type_returns_all_receipt_types()
        {
            var user = await GetAuthenticatedUserAsync();
            var baseDate = new Faker().Date.Past();

            // Add a purchase receipt
            var purchase = PurchaseBuilder
                .Simple()
                .WithPurchasedBy(user)
                .WithStatus(PurchaseStatus.Completed)
                .WithType(PurchaseType.MobilePayV2)
                .WithDateCreated(baseDate)
                .Build();

            // Add a used ticket receipt

            var ticket = TicketBuilder
                .Simple()
                .WithOwner(user)
                .WithStatus(TicketStatus.Used)
                .WithDateUsed(baseDate.AddHours(-2))
                .WithDateCreated(baseDate.AddHours(-1))
                .Build();

            // Add a voucher receipt
            var product = ProductBuilder.Simple().Build();
            await Context.Products.AddAsync(product);

            var voucher = VoucherBuilder
                .Simple()
                .WithUser(user)
                .WithPurchase(
                    PurchaseBuilder
                        .Simple()
                        .WithType(PurchaseType.Voucher)
                        .WithPurchasedBy(user)
                        .Build()
                )
                .Build();

            await Context.Purchases.AddAsync(purchase);
            await Context.Tickets.AddAsync(ticket);
            await Context.Vouchers.AddAsync(voucher);
            await Context.SaveChangesAsync();

            var response = await CoffeeCardClientV2.Receipt_GetReceiptsAsync(
                ReceiptType.All,
                20,
                null
            );

            Assert.Equal(3, response.Receipts.Count);
            Assert.Single(response.Receipts.OfType<PurchaseReceipt>());
            Assert.Single(response.Receipts.OfType<UsedTicketReceipt>());
            Assert.Single(response.Receipts.OfType<VoucherReceipt>());
        }

        [Fact]
        public async Task GetReceipts_with_invalid_continuation_token_returns_400()
        {
            await GetAuthenticatedUserAsync();

            var exception = await Assert.ThrowsAsync<ApiException>(async () =>
                await CoffeeCardClientV2.Receipt_GetReceiptsAsync(
                    ReceiptType.All,
                    20,
                    "not-a-valid-token"
                )
            );

            Assert.Equal(400, exception.StatusCode);
        }
    }
}
