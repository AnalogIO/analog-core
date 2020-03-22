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