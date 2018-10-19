using coffeecard.Models.DataTransferObjects.Programme;
using coffeecard.Models.DataTransferObjects.Purchase;
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

        IEnumerable<PurchaseDTO> Map(IEnumerable<Purchase> purchase);
        IEnumerable<ProgrammeDTO> Map(IEnumerable<Programme> programmes);
    }
}
