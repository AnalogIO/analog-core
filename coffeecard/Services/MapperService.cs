using coffeecard.Models.DataTransferObjects.Purchase;
using Coffeecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
