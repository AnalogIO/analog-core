using System.Collections.Generic;
using CoffeeCard.Common.Models;

namespace CoffeeCard.Library.Services
{
    public interface IProgrammeService
    {
        IEnumerable<Programme> GetProgrammes();
        Programme GetProgramme(int id);
    }
}