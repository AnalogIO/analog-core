using System.Collections.Generic;
using CoffeeCard.Models;
using CoffeeCard.Models.DataTransferObjects.Product;
using CoffeeCard.Models.DataTransferObjects.Programme;
using CoffeeCard.Models.DataTransferObjects.Purchase;
using CoffeeCard.Models.DataTransferObjects.Ticket;
using CoffeeCard.Models.DataTransferObjects.User;

namespace CoffeeCard.Services
{
    public interface IMapperService
    {
        PurchaseDTO Map(Purchase purchase);
        ProgrammeDTO Map(Programme progamme);
        UserDTO Map(User user);
        ProductDTO Map(Product product);
        TicketDTO Map(Ticket ticket);
        IEnumerable<PurchaseDTO> Map(IEnumerable<Purchase> purchase);
        IEnumerable<ProgrammeDTO> Map(IEnumerable<Programme> programmes);
        IEnumerable<ProductDTO> Map(IEnumerable<Product> products);
        IEnumerable<TicketDTO> Map(IEnumerable<Ticket> tickets);
    }
}
