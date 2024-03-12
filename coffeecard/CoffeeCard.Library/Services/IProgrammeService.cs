using CoffeeCard.Models.Entities;
using System.Collections.Generic;

namespace CoffeeCard.Library.Services
{
    public interface IProgrammeService
    {
        IEnumerable<Programme> GetProgrammes();
        Programme GetProgramme(int id);
    }
}