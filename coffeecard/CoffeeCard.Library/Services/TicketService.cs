using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.Common;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Models.DataTransferObjects.CoffeeCard;
using CoffeeCard.Models.DataTransferObjects.Ticket;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.Library.Services
{
    public class TicketService : ITicketService
    {
        private readonly IAccountService _accountService;
        private readonly IProductService _productService;
        private readonly IStatisticService _statisticService;
        private readonly CoffeeCardContext _context;
        private readonly ILogger<TicketService> _logger;

        public TicketService(
            CoffeeCardContext context,
            IAccountService accountService,
            IProductService productService,
            IStatisticService statisticService,
            ILogger<TicketService> logger
        )
        {
            _context = context;
            _accountService = accountService;
            _productService = productService;
            _statisticService = statisticService;
            _logger = logger;
        }

        public IEnumerable<Ticket> GetTickets(IEnumerable<Claim> claims, bool used)
        {
            // (Never return refunded tickets)
            var status = used ? TicketStatus.Used : TicketStatus.Unused;
            var userId = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userId == null)
                throw new ApiException("The token is invalid!", StatusCodes.Status401Unauthorized);
            var id = int.Parse(userId.Value);
            return _context
                .Tickets.Include(p => p.Purchase)
                .Where(x => x.Owner.Id == id && x.Status == status);
        }

        public async Task<Ticket> UseTicket(IEnumerable<Claim> claims, int productId)
        {
            var userIdClaim = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userIdClaim == null)
                throw new ApiException("The token is invalid!", 401);
            var userId = int.Parse(userIdClaim.Value);

            _logger.LogInformation("Using product with id, {productId}", productId);
            var ticketId = GetFirstTicketIdFromProduct(productId, userId);

            _logger.LogInformation("Using ticket with id: {ticketId}", ticketId);
            var usedTicket = ValidateTicket(ticketId, userId);

            UpdateTicket(usedTicket);
            await UpdateUserRank(userId, 1);

            _context.SaveChanges();

            return usedTicket;
        }

        public async Task<IEnumerable<Ticket>> UseMultipleTickets(
            IEnumerable<Claim> claims,
            UseMultipleTicketDto dto
        )
        {
            //Throws exception if the list is empty
            if (!dto.ProductIds.Any())
                throw new ApiException("The list is empty", StatusCodes.Status400BadRequest);

            _logger.LogInformation("Using multiple tickets {@productIds} ", dto.ProductIds);
            var userIdClaim = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userIdClaim == null)
                throw new ApiException("The token is invalid!", StatusCodes.Status401Unauthorized);
            var userId = int.Parse(userIdClaim.Value);

            //Count number of each product
            var groupedProductIds = new Dictionary<int, int>();
            foreach (var productId in dto.ProductIds)
            {
                if (!groupedProductIds.ContainsKey(productId))
                    groupedProductIds.Add(productId, 0);
                groupedProductIds[productId] += 1;
            }

            //First get the tickets from the products used
            var tickets = new List<Ticket>();
            foreach (var keyValue in groupedProductIds)
                tickets.AddRange(
                    GetMultipleTicketsFromProduct(keyValue.Key, userId, keyValue.Value)
                );

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
                UpdateTicket(ticket);
                if (ticket.ProductId != 4)
                    countTicketForRank++;
            }

            await UpdateUserRank(userId, countTicketForRank);

            // only save changes if all tickets was successfully used!
            if (usedTickets.Count == tickets.Count)
            {
                // update user experience
                usedTickets.ForEach(x =>
                    _accountService.UpdateExperience(userId, GetExperienceByTicket(x.ProductId))
                );
                _context.SaveChanges();
                _logger.LogInformation("All tickets were successfully used, updated and saved!");
            }
            else
            {
                _logger.LogError(
                    "All tickets could not be used :-( ticketIds: {@tickets}",
                    tickets
                );
                throw new ApiException(
                    "Could not use the supplied tickets - try again later or contact AnalogIO!",
                    StatusCodes.Status400BadRequest
                );
            }

            return usedTickets;
        }

        public IEnumerable<CoffeeCardDto> GetCoffeeCards(IEnumerable<Claim> claims)
        {
            var userIdClaim = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userIdClaim == null)
                throw new ApiException("The token is invalid!", 401);
            var userId = int.Parse(userIdClaim.Value);
            var user = _context.Users.Find(userId);

            var coffeeCards = _context
                .Tickets.Include(p => p.Purchase)
                .Join(
                    _context.Products,
                    ticket => ticket.ProductId,
                    product => product.Id,
                    (ticket, product) => new { Ticket = ticket, Product = product }
                )
                .Where(tp =>
                    tp.Ticket.Owner.Id == userId && tp.Ticket.Status == TicketStatus.Unused
                )
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
                            TicketsLeft = tp.Count(),
                        }
                )
                .ToList();

            var products = _productService
                .GetProductsForUserAsync(user)
                .Result.Select(p => new Models.DataTransferObjects.CoffeeCard.CoffeeCard
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = p.NumberOfTickets,
                    TicketsLeft = 0,
                })
                .ToList();

            var unionCoffeeCards = coffeeCards.Union(products, new CoffeeCardComparer());
            var toDto = unionCoffeeCards.Select(cc => new CoffeeCardDto()
            {
                ProductId = cc.ProductId,
                Name = cc.Name,
                Price = cc.Price,
                Quantity = cc.Quantity,
                TicketsLeft = cc.TicketsLeft,
            });
            return toDto.ToList();
        }

        private int GetFirstTicketIdFromProduct(int productId, int userId)
        {
            return _context
                .Tickets.Include(p => p.Purchase)
                .FirstOrDefault(x =>
                    x.Owner.Id == userId
                    && x.ProductId == productId
                    && x.Status == TicketStatus.Unused
                )
                .Id;
        }

        private IEnumerable<Ticket> GetMultipleTicketsFromProduct(
            int productId,
            int userId,
            int count
        )
        {
            return _context
                .Tickets.Include(p => p.Purchase)
                .Where(x =>
                    x.Owner.Id == userId
                    && x.ProductId == productId
                    && x.Status == TicketStatus.Unused
                )
                .Take(count);
        }

        private Ticket ValidateTicket(int ticketId, int userId)
        {
            _logger.LogInformation(
                "Validating that ticketId: {ticketId} belongs to userId: {userId} and is not used",
                ticketId,
                userId
            );
            var ticket = _context
                .Tickets.Include(x => x.Purchase)
                .FirstOrDefault(x =>
                    x.Id == ticketId && x.Status == TicketStatus.Unused && x.Owner.Id == userId
                );
            if (ticket == null)
                throw new ApiException("The ticket is invalid", StatusCodes.Status400BadRequest);
            return ticket;
        }

        private void UpdateTicket(Ticket ticket)
        {
            ticket.Status = TicketStatus.Used;
            ticket.DateUsed = DateTime.UtcNow;
        }

        private int GetExperienceByTicket(int productId)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == productId);
            return product?.ExperienceWorth ?? 0;
        }

        private async Task UpdateUserRank(int userId, int tickets)
        {
            await _statisticService.IncreaseStatisticsBy(userId, tickets);
        }
    }
}
