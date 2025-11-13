using System.Collections.Generic;
using CoffeeCard.Models.DataTransferObjects.Product;
using CoffeeCard.Models.DataTransferObjects.Programme;
using CoffeeCard.Models.DataTransferObjects.Purchase;
using CoffeeCard.Models.DataTransferObjects.Ticket;
using CoffeeCard.Models.DataTransferObjects.User;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services
{
    public interface IMapperService
    {
        PurchaseDto Map(Purchase purchase);
        ProgrammeDto Map(Programme progamme);
        UserDto Map(User user);
        ProductDto Map(Product product);
        TicketDto Map(Ticket ticket);
        IEnumerable<PurchaseDto> Map(IEnumerable<Purchase> purchases);
        IEnumerable<ProgrammeDto> Map(IEnumerable<Programme> programmes);
        IEnumerable<ProductDto> Map(IEnumerable<Product> products);
        IEnumerable<TicketDto> Map(IEnumerable<Ticket> tickets);
    }
}
