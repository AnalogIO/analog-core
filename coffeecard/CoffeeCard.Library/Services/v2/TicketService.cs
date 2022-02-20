using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.Entities;

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
    }
}