using System.Collections.Generic;
using CoffeeCard.Models;

namespace CoffeeCard.Services
{
    public interface IProgrammeService
    {
        IEnumerable<Programme> GetProgrammes();
        Programme GetProgramme(int id);
    }
}