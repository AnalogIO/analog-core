using System.Collections.Generic;
using System.Linq;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.Product;
using CoffeeCard.Models.DataTransferObjects.Programme;
using CoffeeCard.Models.DataTransferObjects.Purchase;
using CoffeeCard.Models.DataTransferObjects.Ticket;
using CoffeeCard.Models.DataTransferObjects.User;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services
{
    public class MapperService : IMapperService
    {
        public PurchaseDto Map(Purchase purchase)
        {
            return new PurchaseDto
            {
                Id = purchase.Id,
                ProductName = purchase.ProductName,
                ProductId = purchase.ProductId,
                Price = purchase.Price,
                NumberOfTickets = purchase.NumberOfTickets,
                DateCreated = purchase.DateCreated,
                OrderId = purchase.OrderId,
                TransactionId = purchase.ExternalTransactionId,
            };
        }

        public IEnumerable<PurchaseDto> Map(IEnumerable<Purchase> purchases)
        {
            return purchases.Select(Map);
        }

        public ProgrammeDto Map(Programme progamme)
        {
            return new ProgrammeDto
            {
                Id = progamme.Id,
                ShortName = progamme.ShortName,
                FullName = progamme.FullName,
            };
        }

        public IEnumerable<ProgrammeDto> Map(IEnumerable<Programme> programmes)
        {
            return programmes.Select(Map);
        }

        public UserDto Map(User user)
        {
            return new UserDto
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                PrivacyActivated = user.PrivacyActivated,
                ProgrammeId = user.Programme?.Id,
                Level = StatisticUtils.CalculateLevelFromXp(user.Experience),
                RequiredExp = StatisticUtils.CalculateRequiredXpByLevel(user.Experience),
            };
        }

        public ProductDto Map(Product product)
        {
            return new ProductDto
            {
                Description = product.Description,
                Id = product.Id,
                Name = product.Name,
                NumberOfTickets = product.NumberOfTickets,
                Price = product.Price,
            };
        }

        public IEnumerable<ProductDto> Map(IEnumerable<Product> products)
        {
            return products.Select(Map);
        }

        public TicketDto Map(Ticket ticket)
        {
            return new TicketDto
            {
                Id = ticket.Id,
                DateCreated = ticket.DateCreated,
                DateUsed = ticket.DateUsed,
                ProductName = ticket.Purchase?.ProductName,
            };
        }

        public IEnumerable<TicketDto> Map(IEnumerable<Ticket> tickets)
        {
            return tickets.Select(Map);
        }
    }
}
