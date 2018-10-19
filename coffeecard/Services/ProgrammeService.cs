using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coffeecard.Models;

namespace coffeecard.Services
{
    public class ProgrammeService : IProgrammeService
    {
        private readonly CoffeecardContext _context;

        public ProgrammeService(CoffeecardContext context)
        {
            _context = context;
        }
        public IEnumerable<Programme> GetProgrammes()
        {
            return _context.Programmes.AsEnumerable();
        }
    }
}
