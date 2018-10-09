using coffeecard.Models.DataTransferObjects.Purchase;
using Coffeecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeecard.Services
{
    public interface IMapperService
    {
        PurchaseDTO Map(Purchase purchase);

        IEnumerable<PurchaseDTO> Map(IEnumerable<Purchase> purchase);
    }
}
