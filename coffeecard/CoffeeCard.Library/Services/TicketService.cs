﻿using CoffeeCard.Common;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Models.DataTransferObjects.CoffeeCard;
using CoffeeCard.Models.DataTransferObjects.Ticket;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoffeeCard.Library.Services
{
    public class TicketService : ITicketService
    {
        private readonly IAccountService _accountService;
        private readonly IProductService _productService;
        private readonly IStatisticService _statisticService;
        private readonly CoffeeCardContext _context;

        public TicketService(CoffeeCardContext context, IAccountService accountService, IProductService productService, IStatisticService statisticService)
        {
            _context = context;
            _accountService = accountService;
            _productService = productService;
            _statisticService = statisticService;
        }

        public IEnumerable<Ticket> GetTickets(IEnumerable<Claim> claims, bool used)
        {
            Claim userId = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userId == null) throw new ApiException("The token is invalid!", StatusCodes.Status401Unauthorized);
            int id = int.Parse(userId.Value);
            return _context.Tickets.Include(p => p.Purchase).Where(x => x.Owner.Id == id && x.IsUsed == used);
        }

        public async Task<Ticket> UseTicket(IEnumerable<Claim> claims, int productId)
        {
            Claim userIdClaim = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userIdClaim == null) throw new ApiException("The token is invalid!", 401);
            int userId = int.Parse(userIdClaim.Value);

            Log.Information($"Using product with id, {productId}");
            int ticketId = GetFirstTicketIdFromProduct(productId, userId);

            Log.Information($"Using ticket with id: {ticketId}");
            Ticket usedTicket = ValidateTicket(ticketId, userId);

            UpdateTicket(usedTicket);
            await UpdateUserRank(userId, 1);

            _ = _context.SaveChanges();

            return usedTicket;
        }

        public async Task<IEnumerable<Ticket>> UseMultipleTickets(IEnumerable<Claim> claims, UseMultipleTicketDto dto)
        {
            //Throws exception if the list is empty
            if (!dto.ProductIds.Any()) throw new ApiException("The list is empty", StatusCodes.Status400BadRequest);

            Log.Information($"Using multiple tickets {string.Join(",", dto.ProductIds)}");
            Claim userIdClaim = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userIdClaim == null) throw new ApiException("The token is invalid!", StatusCodes.Status401Unauthorized);
            int userId = int.Parse(userIdClaim.Value);

            //Count number of each product
            Dictionary<int, int> groupedProductIds = [];
            foreach (int productId in dto.ProductIds)
            {
                if (!groupedProductIds.ContainsKey(productId)) groupedProductIds.Add(productId, 0);
                groupedProductIds[productId] += 1;
            }

            //First get the tickets from the products used
            List<Ticket> tickets = [];
            foreach (KeyValuePair<int, int> keyValue in groupedProductIds)
                tickets.AddRange(GetMultipleTicketsFromProduct(keyValue.Key, userId, keyValue.Value));

            //Use the tickets
            List<Ticket> usedTickets = [];
            foreach (Ticket ticket in tickets)
            {
                Ticket usedTicket = ValidateTicket(ticket.Id, userId);
                usedTickets.Add(usedTicket);
            }

            int countTicketForRank = 0;
            foreach (Ticket ticket in usedTickets)
            {
                UpdateTicket(ticket);
                if (ticket.ProductId != 4)
                    countTicketForRank++;
            }

            await UpdateUserRank(userId, countTicketForRank);

            // only save changes if all tickets was successfully used!
            if (usedTickets.Count == tickets.Count)
            {
                // update user experience
                usedTickets.ForEach(x => _accountService.UpdateExperience(userId, GetExperienceByTicket(x.ProductId)));
                _ = _context.SaveChanges();
                Log.Information("All tickets were successfully used, updated and saved!");
            }
            else
            {
                Log.Error($"All tickets could not be used :-( ticketIds: {string.Join(",", tickets)}");
                throw new ApiException("Could not use the supplied tickets - try again later or contact AnalogIO!", StatusCodes.Status400BadRequest);
            }

            return usedTickets;
        }

        public IEnumerable<CoffeeCardDto> GetCoffeeCards(IEnumerable<Claim> claims)
        {
            Claim userIdClaim = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userIdClaim == null) throw new ApiException("The token is invalid!", 401);
            int userId = int.Parse(userIdClaim.Value);
            User user = _context.Users.Find(userId);

            List<Models.DataTransferObjects.CoffeeCard.CoffeeCard> coffeeCards = _context.Tickets
                .Include(p => p.Purchase)
                .Join(_context.Products,
                    ticket => ticket.ProductId,
                    product => product.Id,
                    (ticket, product) => new { Ticket = ticket, Product = product })
                .Where(tp => tp.Ticket.Owner.Id == userId && !tp.Ticket.IsUsed)
                .AsEnumerable()
                .GroupBy(
                    tp => tp.Product,
                    tp => tp.Ticket,
                    (product, tp) =>
                        new Models.DataTransferObjects.CoffeeCard.CoffeeCard
                        {
                            ProductId = product.Id,
                            Name = product.Name,
                            Price = product.Price,
                            Quantity = product.NumberOfTickets,
                            TicketsLeft = tp.Count()
                        }).ToList();

            List<Models.DataTransferObjects.CoffeeCard.CoffeeCard> products = _productService.GetProductsForUserAsync(user).Result.Select(p => new Models.DataTransferObjects.CoffeeCard.CoffeeCard
            { ProductId = p.Id, Name = p.Name, Price = p.Price, Quantity = p.NumberOfTickets, TicketsLeft = 0 }).ToList();

            IEnumerable<Models.DataTransferObjects.CoffeeCard.CoffeeCard> unionCoffeeCards = coffeeCards.Union(products, new CoffeeCardComparer());
            IEnumerable<CoffeeCardDto> toDto = unionCoffeeCards.Select(cc => new CoffeeCardDto()
            {
                ProductId = cc.ProductId,
                Name = cc.Name,
                Price = cc.Price,
                Quantity = cc.Quantity,
                TicketsLeft = cc.TicketsLeft
            });
            return toDto.ToList();
        }

        private int GetFirstTicketIdFromProduct(int productId, int userId)
        {
            return _context.Tickets
                .Include(p => p.Purchase)
                .FirstOrDefault(x => x.Owner.Id == userId && x.ProductId == productId && !x.IsUsed).Id;
        }

        private IEnumerable<Ticket> GetMultipleTicketsFromProduct(int productId, int userId, int count)
        {
            return _context.Tickets.Include(p => p.Purchase)
                .Where(x => x.Owner.Id == userId && x.ProductId == productId && !x.IsUsed)
                .Take(count);
        }

        private Ticket ValidateTicket(int ticketId, int userId)
        {
            Log.Information($"Validating that ticketId: {ticketId} belongs to userId: {userId} and is not used");
            Ticket ticket = _context.Tickets.Include(x => x.Purchase)
                .FirstOrDefault(x => x.Id == ticketId && !x.IsUsed && x.Owner.Id == userId);
            if (ticket == null) throw new ApiException("The ticket is invalid", StatusCodes.Status400BadRequest);
            return ticket;
        }

        private void UpdateTicket(Ticket ticket)
        {
            ticket.IsUsed = true;
            ticket.DateUsed = DateTime.UtcNow;
        }

        private int GetExperienceByTicket(int productId)
        {
            Product product = _context.Products.FirstOrDefault(x => x.Id == productId);
            return product?.ExperienceWorth ?? 0;
        }

        private async Task UpdateUserRank(int userId, int tickets)
        {
            await _statisticService.IncreaseStatisticsBy(userId, tickets);
        }
    }
}