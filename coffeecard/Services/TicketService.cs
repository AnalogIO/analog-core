using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using coffeecard.Controllers;
using coffeecard.Helpers;
using coffeecard.Models.DataTransferObjects.Ticket;
using Coffeecard.Models;
using Microsoft.EntityFrameworkCore;

namespace coffeecard.Services
{
    public class TicketService : ITicketService
    {

        private readonly CoffeecardContext _context;
        private readonly IAccountService _accountService;

        public TicketService(CoffeecardContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        public IEnumerable<Ticket> getTickets(IEnumerable<Claim> claims, bool used)
        {
            var userId = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userId == null) throw new ApiException($"The token is invalid!", 401);
            var id = int.Parse(userId.Value);
            return _context.Tickets.Include(p => p.Purchase).Where(x => x.Owner.Id == id && x.IsUsed == used);
        }
        
        public Ticket UseTicket(IEnumerable<Claim> claims, int ticketId)
        {
            var userIdClaim = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userIdClaim == null) throw new ApiException($"The token is invalid!", 401);
            var userId = int.Parse(userIdClaim.Value);

            var usedTicket = ValidateTicket(ticketId, userId);

            UpdateTicket(usedTicket, userId);

            UpdateUserRank(userId, 1);

            return usedTicket;
        }

        public IEnumerable<Ticket> UseMultipleTickets(IEnumerable<Claim> claims, int[] ticketIds)
        {
            var userIdClaim = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userIdClaim == null) throw new ApiException($"The token is invalid!", 401);
            var userId = int.Parse(userIdClaim.Value);

            var usedTickets = new List<Ticket>();
            foreach(int ticketId in ticketIds)
            {
                var usedTicket = ValidateTicket(ticketId, userId);
                usedTickets.Add(usedTicket);
            }

            var countTicketForRank = 0;
            foreach(var ticket in usedTickets)
            {
                UpdateTicket(ticket, userId);
                if (ticket.ProductId != 4)
                    countTicketForRank++;
            }

            UpdateUserRank(userId, countTicketForRank);

            return usedTickets;
        }

        private Ticket ValidateTicket(int ticketId, int userId)
        {
            var usedTicket = _context.Tickets.FirstOrDefault(x => x.Id == ticketId && !x.IsUsed && x.Owner.Id == userId);
            if (usedTicket == null) throw new ApiException("User don't own ticket", 400);
            return usedTicket;
        }

        private void UpdateTicket(Ticket ticket, int userId)
        {
            ticket.IsUsed = true;
            ticket.DateUsed = DateTime.UtcNow;
            _context.SaveChanges();

            _accountService.UpdateExperience(userId, GetExperienceByTicket(ticket.ProductId));
        }

        private int GetExperienceByTicket(int productId)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == productId);
            return product != null ? product.ExperienceWorth : 0;
        }

        private void UpdateUserRank(int userId, int tickets)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            user.IncreaseStatisticsBy(tickets);
        }
    }
}
