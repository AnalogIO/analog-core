using System.Collections.Generic;
using CoffeeCard.WebApi.Models;
using CoffeeCard.WebApi.Models.DataTransferObjects.Product;
using CoffeeCard.WebApi.Models.DataTransferObjects.Programme;
using CoffeeCard.WebApi.Models.DataTransferObjects.Purchase;
using CoffeeCard.WebApi.Models.DataTransferObjects.Ticket;
using CoffeeCard.WebApi.Models.DataTransferObjects.User;

namespace CoffeeCard.WebApi.Services
{
    public interface IMapperService
    {
        PurchaseDTO Map(Purchase purchase);
        ProgrammeDTO Map(Programme progamme);
        UserDTO Map(User user);
        ProductDTO Map(Product product);
        TicketDTO Map(Ticket ticket);
        IEnumerable<PurchaseDTO> Map(IEnumerable<Purchase> purchases);
        IEnumerable<ProgrammeDTO> Map(IEnumerable<Programme> programmes);
        IEnumerable<ProductDTO> Map(IEnumerable<Product> products);
        IEnumerable<TicketDTO> Map(IEnumerable<Ticket> tickets);
    }
}