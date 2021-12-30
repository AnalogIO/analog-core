using System.Collections.Generic;
using CoffeeCard.Common.Models;
using CoffeeCard.Common.Models.DataTransferObjects.Product;
using CoffeeCard.Common.Models.DataTransferObjects.Programme;
using CoffeeCard.Common.Models.DataTransferObjects.Purchase;
using CoffeeCard.Common.Models.DataTransferObjects.Ticket;
using CoffeeCard.Common.Models.DataTransferObjects.User;

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