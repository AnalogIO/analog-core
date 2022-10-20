﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.DataTransferObjects.Ticket;
using CoffeeCard.Models.DataTransferObjects.v2.Ticket;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CoffeeCard.Library.Services.v2
{
    public class TicketService : ITicketService
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
        
        public async Task<TicketDto> UseTicket(int userId, int productId)
        {
            Log.Information($"Using ticket with id, {productId}");
            var ticket = GetFirstTicketFromProduct(productId, userId);

            ticket.IsUsed = true;
            ticket.DateUsed = DateTime.UtcNow;
            
            if (ticket.Purchase.Price != 0) //Paid products increases your rank on the leaderboard
            {
                await _statisticService.IncreaseStatisticsBy(userId, 1);
            }

            await _context.SaveChangesAsync();

            return new TicketDto
            {
                Id = ticket.Id,
                DateCreated = ticket.DateCreated,
                DateUsed = ticket.DateUsed,
                ProductName = ticket.Purchase.ProductName
            };
        }
        
        private Ticket GetFirstTicketFromProduct(int productId, int userId)
        {
            var ticket = _context.Tickets
                .Include(t => t.Purchase)
                .FirstOrDefault(t => t.Owner.Id == userId && t.ProductId == productId && !t.IsUsed);
            
            if (ticket == null)
            {
                throw new ApiException("No tickets found for the given product with this user", StatusCodes.Status404NotFound);
            }
            return ticket;
        }

        private async Task UpdateUserRank(int userId, int tickets)
        {
        }
    }
}