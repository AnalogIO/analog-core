using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.DataTransferObjects.v2.Ticket;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CoffeeCard.Library.Services.v2
{
    public sealed class TicketService : ITicketService
    {
        private readonly CoffeeCardContext _context;
        private readonly IStatisticService _statisticService;

        public TicketService(CoffeeCardContext context, IStatisticService statisticService)
        {
            _context = context;
            _statisticService = statisticService;
        }

        public async Task IssueTickets(Purchase purchase)
        {
            var tickets = new List<Ticket>();
            for (var i = 0; i < purchase.NumberOfTickets; i++)
            {
                tickets.Add(new Ticket
                {
                    DateCreated = DateTime.UtcNow,
                    ProductId = purchase.ProductId,
                    IsUsed = false,
                    Owner = purchase.PurchasedBy,
                    Purchase = purchase
                });
            }

            await _context.Tickets.AddRangeAsync(tickets);
            await _context.SaveChangesAsync();

            Log.Information("Issued {NoTickets} Tickets for ProductId {ProductId}, PurchaseId {PurchaseId}", purchase.NumberOfTickets, purchase.ProductId, purchase.Id);
        }

        public Task<List<TicketResponse>> GetTickets(User user, bool includeUsed)
        {
            return _context.Tickets
                .Where(t => t.Owner.Equals(user) && t.IsUsed == includeUsed)
                .Include(t => t.Purchase)
                .Include(t => t.UsedOnMenuItem)
                .Select(t => new TicketResponse
                {
                    Id = t.Id,
                    DateCreated = t.DateCreated,
                    DateUsed = t.DateUsed,
                    ProductId = t.ProductId,
                    ProductName = t.Purchase.ProductName,
                    UsedOnMenuItemName = t.UsedOnMenuItem != null ? t.UsedOnMenuItem.Name : null
                }).ToListAsync();
        }

        public async Task<UsedTicketResponse> UseTicketAsync(User user, int productId)
        {
            Log.Information("UserId {UserId} uses a ticket for ProductId {ProductId}", user.Id, productId);

            var ticket = await GetFirstTicketFromProductAsync(productId, user.Id);

            ticket.IsUsed = true;
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
                ProductName = ticket.Purchase.ProductName
            };
        }

        public async Task<UsedTicketResponse> UseTicketAsync(User user, int productId, int menuItemId)
        {
            Log.Information($"UserId {user.Id} uses a ticket for MenuItemId {menuItemId} via ProductId {productId}");

            var ticket = await GetFirstTicketFromProductAsync(productId, user.Id);
            var menuItem = await GetMenuItemFromProductAsync(menuItemId, productId);

            ticket.IsUsed = true;
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
                MenuItemName = menuItem.Name
            };
        }

        private async Task<Ticket> GetFirstTicketFromProductAsync(int productId, int userId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Purchase)
                .FirstOrDefaultAsync(t => t.Owner.Id == userId && t.ProductId == productId && !t.IsUsed);

            if (ticket == null)
            {
                throw new EntityNotFoundException("No tickets found for the given product with this user");
            }
            return ticket;
        }

        private async Task<MenuItem> GetMenuItemFromProductAsync(int menuItemId, int productId)
        {
            var menuItem = await _context.MenuItems
                .Include(m => m.Products)
                .FirstOrDefaultAsync(m => m.Id == menuItemId);

            if (menuItem == null)
            {
                throw new EntityNotFoundException("The menu item was not found");
            }

            if (!menuItem.Products.Any(p => p.Id == productId))
            {
                throw new IllegalUserOperationException(
                    $"Product is not eligible to redeem the menu item '{menuItem.Name}'"
                );
            }

            return menuItem;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}