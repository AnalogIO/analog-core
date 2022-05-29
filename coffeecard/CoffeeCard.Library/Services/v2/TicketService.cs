using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.DataTransferObjects.v2.Ticket;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Library.Services.v2
{
    public class TicketService : ITicketService
    {
        private readonly CoffeeCardContext _context;

        public TicketService(CoffeeCardContext context)
        {
            _context = context;
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
        }

        public Task<List<TicketResponse>> GetTickets(User user, bool includeUsed)
        {
            return _context.Tickets.Where(t => t.Owner.Equals(user) && t.IsUsed == includeUsed).Include(t => t.Purchase)
                .Select(t => new TicketResponse
                {
                    Id = t.Id,
                    DateCreated = t.DateCreated,
                    DateUsed = t.DateUsed,
                    ProductId = t.ProductId,
                    ProductName = t.Purchase.ProductName
                }).ToListAsync();
        }
    }
}