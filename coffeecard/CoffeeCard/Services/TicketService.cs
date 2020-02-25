using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using CoffeeCard.Helpers;
using CoffeeCard.Models;
using CoffeeCard.Models.DataTransferObjects.Ticket;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CoffeeCard.Services
{
    public class TicketService : ITicketService
    {
        private readonly IAccountService _accountService;

        private readonly CoffeeCardContext _context;

        public TicketService(CoffeeCardContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        public IEnumerable<Ticket> getTickets(IEnumerable<Claim> claims, bool used)
        {
            var userId = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userId == null) throw new ApiException("The token is invalid!", 401);
            var id = int.Parse(userId.Value);
            return _context.Tickets.Include(p => p.Purchase).Where(x => x.Owner.Id == id && x.IsUsed == used);
        }

        public Ticket UseTicket(IEnumerable<Claim> claims, int productId)
        {
            var userIdClaim = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userIdClaim == null) throw new ApiException("The token is invalid!", 401);
            var userId = int.Parse(userIdClaim.Value);

            Log.Information($"Using product with id, {productId}");
            var ticketId = GetFirstTicketIdFromProduct(productId, userId);

            Log.Information($"Using ticket with id: {ticketId}");
            var usedTicket = ValidateTicket(ticketId, userId);

            UpdateTicket(usedTicket, userId);
            UpdateUserRank(userId, 1);

            _context.SaveChanges();

            return usedTicket;
        }

        public IEnumerable<Ticket> UseMultipleTickets(IEnumerable<Claim> claims, UseMultipleTicketDTO dto)
        {
            //Throws exception if the list is empty
            if (dto.ProductIds.Count() == 0) throw new ApiException("The list is empty", 400);

            Log.Information($"Using multiple tickets {string.Join(",", dto.ProductIds)}");
            var userIdClaim = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userIdClaim == null) throw new ApiException("The token is invalid!", 401);
            var userId = int.Parse(userIdClaim.Value);

            //Count number of each product
            var groupedProductIds = new Dictionary<int, int>();
            foreach (var productId in dto.ProductIds)
            {
                if (!groupedProductIds.ContainsKey(productId)) groupedProductIds.Add(productId, 0);
                groupedProductIds[productId] += 1;
            }

            //First get the tickets from the products used
            var tickets = new List<Ticket>();
            foreach (var keyValye in groupedProductIds)
                tickets.AddRange(GetMultipleTicketsFromProduct(keyValye.Key, userId, keyValye.Value));

            //Use the tickets
            var usedTickets = new List<Ticket>();
            foreach (var ticket in tickets)
            {
                var usedTicket = ValidateTicket(ticket.Id, userId);
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
            if (usedTickets.Count == tickets.Count())
            {
                // update user experience
                usedTickets.ForEach(x => _accountService.UpdateExperience(userId, GetExperienceByTicket(x.ProductId)));
                _context.SaveChanges();
                Log.Information("All tickets were successfully used, updated and saved!");
            }
            else
            {
                Log.Error($"All tickets could not be used :-( ticketIds: {string.Join(",", tickets)}");
                throw new ApiException("Could not use the supplied tickets - try again later or contact AnalogIO!");
            }

            return usedTickets;
        }

        public IEnumerable<CoffeCard> GetCoffeCards(IEnumerable<Claim> claims)
        {
            var userIdClaim = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userIdClaim == null) throw new ApiException("The token is invalid!", 401);
            var userId = int.Parse(userIdClaim.Value);

            var coffeCards = _context.Tickets
                .Include(p => p.Purchase)
                .Join(_context.Products,
                    ticket => ticket.ProductId,
                    product => product.Id,
                    (ticket, product) => new {Ticket = ticket, Product = product})
                .Where(tp => tp.Ticket.Owner.Id == userId && tp.Ticket.IsUsed == false)
                .GroupBy(
                    tp => tp.Product,
                    tp => tp.Ticket,
                    (product, tp) =>
                        new CoffeCard
                        {
                            ProductId = product.Id,
                            Name = product.Name,
                            Price = product.Price,
                            Quantity = product.NumberOfTickets,
                            TicketsLeft = tp.Count()
                        }).ToList();

            var products = _context.Products.Select(p => new CoffeCard
                {ProductId = p.Id, Name = p.Name, Price = p.Price, Quantity = p.NumberOfTickets, TicketsLeft = 0});

            return coffeCards.Union(products, new CoffeeCardComparer());
        }

        private int GetFirstTicketIdFromProduct(int productId, int userId)
        {
            return _context.Tickets.Include(p => p.Purchase)
                .Where(x => x.Owner.Id == userId && x.ProductId == productId && x.IsUsed == false)
                .FirstOrDefault().Id;
        }

        private IEnumerable<Ticket> GetMultipleTicketsFromProduct(int productId, int userId, int count)
        {
            return _context.Tickets.Include(p => p.Purchase)
                .Where(x => x.Owner.Id == userId && x.ProductId == productId && x.IsUsed == false)
                .Take(count);
        }

        private Ticket ValidateTicket(int ticketId, int userId)
        {
            Log.Information($"Validating that ticketId: {ticketId} belongs to userId: {userId} and is not used");
            var ticket = _context.Tickets.Include(x => x.Purchase)
                .FirstOrDefault(x => x.Id == ticketId && !x.IsUsed && x.Owner.Id == userId);
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