using coffeecard.Models.DataTransferObjects.Product;
using coffeecard.Models.DataTransferObjects.Programme;
using coffeecard.Models.DataTransferObjects.Purchase;
using coffeecard.Models.DataTransferObjects.Ticket;
using coffeecard.Models.DataTransferObjects.User;
using Coffeecard.Models;
using System.Collections.Generic;
using System.Linq;

namespace coffeecard.Services
{
    public class MapperService : IMapperService
    {
        public PurchaseDTO Map(Purchase purchase)
        {
            return new PurchaseDTO
            {
                Id = purchase.Id,
                ProductName = purchase.ProductName,
                ProductId = purchase.ProductId,
                Price = purchase.Price,
                NumberOfTickets = purchase.NumberOfTickets,
                DateCreated = purchase.DateCreated,
                Completed = purchase.Completed,
                OrderId = purchase.OrderId,
                TransactionId = purchase.TransactionId
            };
        }

        public IEnumerable<PurchaseDTO> Map(IEnumerable<Purchase> purchases)
        {
            return purchases.Select(Map);
        }

        public ProgrammeDTO Map(Programme progamme)
        {
            return new ProgrammeDTO
            {
                Id = progamme.Id,
                ShortName = progamme.ShortName,
                FullName = progamme.FullName
            };
        }

        public IEnumerable<ProgrammeDTO> Map(IEnumerable<Programme> programmes)
        {
            return programmes.Select(Map);
        }

        public UserDTO Map(User user)
        {


            return new UserDTO { Email = user.Email, Id = user.Id, Name = user.Name, PrivacyActivated = user.PrivacyActivated, ProgrammeId = user.Programme.Id, Level = user.CalculateLevelFromXp(), RequiredExp = user.CalculateRequiredXpByLevel() };
        }

        public ProductDTO Map(Product product)
        {
            return new ProductDTO { Description = product.Description, Id = product.Id, Name = product.Name, NumberOfTickets = product.NumberOfTickets, Price = product.Price };
        }

        public IEnumerable<ProductDTO> Map(IEnumerable<Product> products)
        {
            return products.Select(Map);
        }

        public TicketDTO Map(Ticket ticket)
        {
            return new TicketDTO { Id = ticket.Id, DateCreated = ticket.DateCreated, DateUsed = ticket.DateUsed, ProductName = ticket.Purchase?.ProductName };
        }

        public IEnumerable<TicketDTO> Map(IEnumerable<Ticket> tickets)
        {
            return tickets.Select(Map);
        }
    }
}
