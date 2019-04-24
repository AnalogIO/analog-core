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
using Serilog;

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

        private int GetTicketIdFromProduct(int productId, int userId)
        {
            return _context.Tickets.Include(p => p.Purchase)
                .Where(x => x.Owner.Id == userId && x.ProductId == productId && x.IsUsed == false)
                .FirstOrDefault().Id;
        }

        public Ticket UseTicket(IEnumerable<Claim> claims, int productId)
        {
            var userIdClaim = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userIdClaim == null) throw new ApiException($"The token is invalid!", 401);
            var userId = int.Parse(userIdClaim.Value);

            Log.Information($"Using product with id, {productId}");
            int ticketId = GetTicketIdFromProduct(productId, userId);

            Log.Information($"Using ticket with id: {ticketId}");
            var usedTicket = ValidateTicket(ticketId, userId);

            UpdateTicket(usedTicket, userId);
            UpdateUserRank(userId, 1);

            return usedTicket;
        }

        public IEnumerable<Ticket> UseMultipleTickets(IEnumerable<Claim> claims, int[] ticketIds)
        {
            Log.Information($"Using multiple tickets {string.Join(",", ticketIds)}");
            var userIdClaim = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userIdClaim == null) throw new ApiException($"The token is invalid!", 401);
            var userId = int.Parse(userIdClaim.Value);

            var usedTickets = new List<Ticket>();
            foreach (int ticketId in ticketIds)
            {
                var usedTicket = ValidateTicket(ticketId, userId);
                usedTickets.Add(usedTicket);
            }

            var countTicketForRank = 0;
            foreach (var ticket in usedTickets)
            {
                UpdateTicket(ticket, userId);
                if (ticket.ProductId != 4)
                    countTicketForRank++;
            }

            UpdateUserRank(userId, countTicketForRank);

            // only save changes if all tickets was successfully used!
            if (usedTickets.Count == ticketIds.Count())
            {
                // update user experience
                usedTickets.ForEach(x => _accountService.UpdateExperience(userId, GetExperienceByTicket(x.ProductId)));
                _context.SaveChanges();
                Log.Information($"All tickets were successfully used, updated and saved!");
            }
            else
            {
                Log.Error($"All tickets could not be used :-( ticketIds: {string.Join(",", ticketIds)}");
                throw new ApiException($"Could not use the supplied tickets - try again later or contact AnalogIO!", 500);
            }

            return usedTickets;
        }

        private Ticket ValidateTicket(int ticketId, int userId)
        {
            Log.Information($"Validating that ticketId: {ticketId} belongs to userId: {userId} and is not used");
            var ticket = _context.Tickets.Include(x => x.Purchase).FirstOrDefault(x => x.Id == ticketId && !x.IsUsed && x.Owner.Id == userId);
            if (ticket == null) throw new ApiException("The ticket is invalid", 400);
            return ticket;
        }

        private void UpdateTicket(Ticket ticket, int userId)
        {
            ticket.IsUsed = true;
            ticket.DateUsed = DateTime.UtcNow;
        }

        private int GetExperienceByTicket(int productId)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == productId);
            return product != null ? product.ExperienceWorth : 0;
        }

        private void UpdateUserRank(int userId, int tickets)
        {
            var user = _context.Users.Include(x => x.Statistics).FirstOrDefault(x => x.Id == userId);
            user.IncreaseStatisticsBy(tickets);
        }
    }
}
