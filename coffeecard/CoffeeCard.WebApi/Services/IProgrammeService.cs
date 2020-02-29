using System.Collections.Generic;
using CoffeeCard.WebApi.Models;

namespace CoffeeCard.WebApi.Services
{
    public interface IProgrammeService
    {
        IEnumerable<Programme> GetProgrammes();
        Programme GetProgramme(int id);
    }
}