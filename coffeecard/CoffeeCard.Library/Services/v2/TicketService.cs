using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.DataTransferObjects.v2.Ticket;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.Library.Services.v2
{
    public sealed class TicketService : ITicketService
    {
        private readonly CoffeeCardContext _context;
        private readonly IStatisticService _statisticService;
        private readonly ILogger<TicketService> _logger;

        public TicketService(
            CoffeeCardContext context,
            IStatisticService statisticService,
            ILogger<TicketService> logger
        )
        {
            _context = context;
            _statisticService = statisticService;
            _logger = logger;
        }

        public async Task IssueTickets(Purchase purchase)
        {
            var tickets = new List<Ticket>();
            for (var i = 0; i < purchase.NumberOfTickets; i++)
            {
                tickets.Add(
                    new Ticket
                    {
                        DateCreated = DateTime.UtcNow,
                        ProductId = purchase.ProductId,
                        Status = TicketStatus.Unused,
                        Owner = purchase.PurchasedBy,
                        Purchase = purchase,
                    }
                );
            }

            await _context.Tickets.AddRangeAsync(tickets);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Issued {NoTickets} Tickets for ProductId {ProductId}, PurchaseId {PurchaseId}",
                purchase.NumberOfTickets,
                purchase.ProductId,
                purchase.Id
            );
        }

        public async Task<IEnumerable<TicketResponse>> GetTicketsAsync(User user, bool includeUsed)
        {
            // (Never return refunded tickets)
            var status = includeUsed ? TicketStatus.Used : TicketStatus.Unused;
            return await _context
                .Tickets.Where(t => t.Owner.Equals(user) && t.Status == status)
                .Include(t => t.Purchase)
                .Include(t => t.UsedOnMenuItem)
                .Select(t => new TicketResponse
                {
                    Id = t.Id,
                    DateCreated = t.DateCreated,
                    DateUsed = t.DateUsed,
                    ProductId = t.ProductId,
                    ProductName = t.Purchase.ProductName,
                    UsedOnMenuItemName = t.UsedOnMenuItem != null ? t.UsedOnMenuItem.Name : null,
                })
                .ToListAsync();
        }

        public async Task<UsedTicketResponse> UseTicketAsync(User user, int productId)
        {
            _logger.LogInformation(
                "UserId {UserId} uses a ticket for ProductId {ProductId}",
                user.Id,
                productId
            );

            var product = await GetProductIncludingMenuItemsFromIdAsync(productId);
            var ticket = await GetFirstTicketFromProductAsync(product, user.Id);

            ticket.Status = TicketStatus.Used;
            var timeUsed = DateTime.UtcNow;
            ticket.DateUsed = timeUsed;

            if (ticket.Purchase.Price > 0) //Paid products increases your rank on the leaderboard
            {
                await _statisticService.IncreaseStatisticsBy(user.Id, 1);
            }

            await _context.SaveChangesAsync();

            return new UsedTicketResponse
            {
                Id = ticket.Id,
                DateCreated = ticket.DateCreated,
                DateUsed = timeUsed,
                ProductName = ticket.Purchase.ProductName,
            };
        }

        public async Task<UsedTicketResponse> UseTicketAsync(
            User user,
            int productId,
            int menuItemId
        )
        {
            _logger.LogInformation(
                "UserId {userId} uses a ticket for MenuItemId {menuItemId} via ProductId {productId}",
                user.Id,
                menuItemId,
                productId
            );

            var product = await GetProductIncludingMenuItemsFromIdAsync(productId);
            var ticket = await GetFirstTicketFromProductAsync(product, user.Id);
            var menuItem = await GetMenuItemByIdAsync(menuItemId);

            if (!product.EligibleMenuItems.Any(mi => mi.Id == menuItem.Id))
            {
                throw new IllegalUserOperationException(
                    "This ticket cannot be used on this menu item"
                );
            }

            ticket.Status = TicketStatus.Used;
            var timeUsed = DateTime.UtcNow;
            ticket.DateUsed = timeUsed;
            ticket.UsedOnMenuItemId = menuItemId;

            if (ticket.Purchase.Price > 0) //Paid products increases your rank on the leaderboard
            {
                await _statisticService.IncreaseStatisticsBy(user.Id, 1);
            }

            await _context.SaveChangesAsync();

            return new UsedTicketResponse
            {
                Id = ticket.Id,
                DateCreated = ticket.DateCreated,
                DateUsed = timeUsed,
                ProductName = ticket.Purchase.ProductName,
                MenuItemName = menuItem.Name,
            };
        }

        private async Task<Product> GetProductIncludingMenuItemsFromIdAsync(int productId)
        {
            return await _context
                    .Products.Include(p => p.EligibleMenuItems)
                    .FirstOrDefaultAsync(p => p.Id == productId)
                ?? throw new EntityNotFoundException("Product not found");
        }

        private async Task<Ticket> GetFirstTicketFromProductAsync(Product product, int userId)
        {
            var user =
                await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new EntityNotFoundException("User not found");

            var ticket =
                await _context
                    .Tickets.Include(t => t.Purchase)
                    .FirstOrDefaultAsync(t =>
                        t.Owner.Id == user.Id
                        && t.ProductId == product.Id
                        && t.Status == TicketStatus.Unused
                    )
                ?? throw new IllegalUserOperationException("User has no tickets for this product");

            return ticket;
        }

        private async Task<MenuItem> GetMenuItemByIdAsync(int menuItemId)
        {
            var menuItem =
                await _context.MenuItems.FirstOrDefaultAsync(m => m.Id == menuItemId)
                ?? throw new EntityNotFoundException("Menu item not found");

            return menuItem;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
