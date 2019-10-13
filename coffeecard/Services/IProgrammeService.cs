using Coffeecard.Models;
using System.Collections.Generic;

namespace coffeecard.Services
{
    public interface IProgrammeService
    {
        IEnumerable<Programme> GetProgrammes();
        Programme GetProgramme(int id);
    }
}
