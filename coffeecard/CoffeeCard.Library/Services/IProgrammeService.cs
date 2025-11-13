using System.Collections.Generic;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services
{
    public interface IProgrammeService
    {
        IEnumerable<Programme> GetProgrammes();
        Programme GetProgramme(int id);
    }
}
