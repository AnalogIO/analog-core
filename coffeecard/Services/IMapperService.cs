using coffeecard.Models.DataTransferObjects.Product;
using coffeecard.Models.DataTransferObjects.Programme;
using coffeecard.Models.DataTransferObjects.Purchase;
using coffeecard.Models.DataTransferObjects.Ticket;
using coffeecard.Models.DataTransferObjects.User;
using Coffeecard.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeecard.Services
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
