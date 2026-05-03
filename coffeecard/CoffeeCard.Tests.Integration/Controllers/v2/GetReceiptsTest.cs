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
using PurchaseStatus = CoffeeCard.Models.DataTransferObjects.v2.Purchase.PurchaseStatus;

namespace CoffeeCard.Tests.Integration.Controllers.v2
{
    public class GetReceiptsTest(CustomWebApplicationFactory<Startup> factory)
        : BaseIntegrationTest(factory)
    {
        // ── Authentication ────────────────────────────────────────────────────

        [Fact]
        public async Task GetReceipts_returns_401_when_not_authenticated()
        {
            RemoveRequestHeaders();

            var exception = await Assert.ThrowsAsync<ApiException>(async () =>
                await CoffeeCardClientV2.Receipt_GetReceiptsAsync()
            );

            Assert.Equal(401, exception.StatusCode);
        }

        // ── Empty state ───────────────────────────────────────────────────────

        [Fact]
        public async Task GetReceipts_returns_empty_list_when_user_has_no_activity()
        {
            await GetAuthenticatedUserAsync();

            var response = await CoffeeCardClientV2.Receipt_GetReceiptsAsync();

            Assert.NotNull(response);
            Assert.Empty(response.Receipts);
        }

        // ── Purchase receipts ─────────────────────────────────────────────────

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

            var response = await CoffeeCardClientV2.Receipt_GetReceiptsAsync();

            Assert.Single(response.Receipts);
            var receipt = response.Receipts.First();
            Assert.StartsWith("Purchase:", receipt.Id);
            Assert.Equal(ReceiptType.Purchase, receipt.Type);
            Assert.Equal(purchase.ProductName, receipt.TicketName);
            Assert.Equal(purchase.NumberOfTickets, receipt.Amount);
            Assert.Equal(purchase.Price, receipt.PriceDKK);
            Assert.Null(receipt.DrinkName);
        }

        // ── Voucher receipts ──────────────────────────────────────────────────

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

            var response = await CoffeeCardClientV2.Receipt_GetReceiptsAsync();

            Assert.Single(response.Receipts);
            var receipt = response.Receipts.First();
            Assert.StartsWith("Voucher:", receipt.Id);
            Assert.Equal(ReceiptType.Voucher, receipt.Type);
            Assert.Equal(purchase.ProductName, receipt.TicketName);
            Assert.Equal(purchase.NumberOfTickets, receipt.Amount);
            Assert.Null(receipt.PriceDKK);
            Assert.Null(receipt.DrinkName);
        }

        // ── UsedTicket receipts ───────────────────────────────────────────────

        [Fact]
        public async Task GetReceipts_returns_used_ticket_receipt_for_used_ticket()
        {
            var user = await GetAuthenticatedUserAsync();
            var swipeDate = new Faker().Date.Past().ToUniversalTime();

            var menuItem = MenuItemBuilder.Simple().Build();
            await Context.MenuItems.AddAsync(menuItem);
            await Context.SaveChangesAsync();

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
                        .WithUsedOnMenuItem(menuItem)
                        .Build(1)
                )
                .Build();

            await Context.Purchases.AddAsync(purchase);
            await Context.SaveChangesAsync();

            var response = await CoffeeCardClientV2.Receipt_GetReceiptsAsync();

            Assert.Single(response.Receipts);
            var receipt = response.Receipts.First();
            Assert.StartsWith("UsedTicket:", receipt.Id);
            Assert.Equal(ReceiptType.UsedTicket, receipt.Type);
            Assert.Equal(menuItem.Name, receipt.DrinkName);
            Assert.Null(receipt.Amount);
            Assert.Null(receipt.PriceDKK);
        }

        [Fact]
        public async Task GetReceipts_used_ticket_without_menu_item_has_null_drink_name()
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
                        .WithUsedOnMenuItem(_ => null)
                        .Build(1)
                )
                .Build();

            await Context.Purchases.AddAsync(purchase);
            await Context.SaveChangesAsync();

            var response = await CoffeeCardClientV2.Receipt_GetReceiptsAsync();

            Assert.Single(response.Receipts);
            Assert.Null(response.Receipts.First().DrinkName);
        }

        // ── User isolation ────────────────────────────────────────────────────

        [Fact]
        public async Task GetReceipts_only_returns_receipts_for_authenticated_user()
        {
            var user = await GetAuthenticatedUserAsync();

            // Another user's purchase — should NOT appear
            var otherPurchase = PurchaseBuilder.Simple().WithType(PurchaseType.MobilePayV2).Build();

            // This user's purchase
            var myPurchase = PurchaseBuilder
                .Simple()
                .WithPurchasedBy(user)
                .WithType(PurchaseType.MobilePayV2)
                .Build();

            await Context.Purchases.AddRangeAsync(otherPurchase, myPurchase);
            await Context.SaveChangesAsync();

            var response = await CoffeeCardClientV2.Receipt_GetReceiptsAsync();

            Assert.Single(response.Receipts);
            Assert.Equal(myPurchase.ProductName, response.Receipts.First().TicketName);
        }

        // ── All-types filter ──────────────────────────────────────────────────

        [Fact]
        public async Task GetReceipts_with_All_type_returns_all_receipt_types()
        {
            var user = await GetAuthenticatedUserAsync();
            var baseDate = new DateTime(2026, 1, 10, 12, 0, 0, DateTimeKind.Utc);

            // Purchase
            var purchase = PurchaseBuilder
                .Simple()
                .WithPurchasedBy(user)
                .WithStatus(PurchaseStatus.Completed)
                .WithType(PurchaseType.MobilePayV2)
                .WithDateCreated(baseDate)
                .WithTickets(new List<Ticket>())
                .WithVoucher(f => null)
                .Build();

            // Voucher purchase
            var voucherPurchase = PurchaseBuilder
                .Simple()
                .WithPurchasedBy(user)
                .WithStatus(PurchaseStatus.Completed)
                .WithType(PurchaseType.Voucher)
                .WithDateCreated(baseDate.AddHours(-1))
                .WithTickets(new List<Ticket>())
                .Build();

            // Used ticket (no menu item)
            var usedPurchase = PurchaseBuilder
                .Simple()
                .WithPurchasedBy(user)
                .WithStatus(PurchaseStatus.Completed)
                .WithType(PurchaseType.Free)
                .WithDateCreated(baseDate.AddHours(-2))
                .WithTickets(
                    TicketBuilder
                        .Simple()
                        .WithOwner(user)
                        .WithStatus(TicketStatus.Used)
                        .WithDateUsed(baseDate.AddHours(-2))
                        .WithUsedOnMenuItem(_ => null)
                        .Build(1)
                )
                .WithVoucher(f => null)
                .Build();

            await Context.Purchases.AddRangeAsync(purchase, voucherPurchase, usedPurchase);
            await Context.SaveChangesAsync();

            var response = await CoffeeCardClientV2.Receipt_GetReceiptsAsync();

            Assert.Equal(3, response.Receipts.Count);

            var types = response.Receipts.Select(r => r.Type).ToHashSet();
            Assert.Contains(ReceiptType.Purchase, types);
            Assert.Contains(ReceiptType.Voucher, types);
            Assert.Contains(ReceiptType.UsedTicket, types);
        }

        // ── Sort order ────────────────────────────────────────────────────────

        [Fact]
        public async Task GetReceipts_items_are_sorted_newest_first()
        {
            var user = await GetAuthenticatedUserAsync();

            var dates = new[]
            {
                new DateTime(2026, 1, 3, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc),
            };

            foreach (var date in dates)
            {
                var p = PurchaseBuilder
                    .Simple()
                    .WithPurchasedBy(user)
                    .WithType(PurchaseType.MobilePayV2)
                    .WithStatus(PurchaseStatus.Completed)
                    .WithDateCreated(date)
                    .WithTickets(new List<Ticket>())
                    .WithVoucher(f => null)
                    .Build();

                await Context.Purchases.AddAsync(p);
            }

            await Context.SaveChangesAsync();

            var response = await CoffeeCardClientV2.Receipt_GetReceiptsAsync();

            Assert.Equal(3, response.Receipts.Count);

            var eventDates = response.Receipts.Select(r => r.EventDate).ToList();
            for (var i = 0; i < eventDates.Count - 1; i++)
            {
                Assert.True(
                    eventDates[i] >= eventDates[i + 1],
                    $"Expected item {i} ({eventDates[i]}) to be >= item {i + 1} ({eventDates[i + 1]})"
                );
            }
        }
    }
}
