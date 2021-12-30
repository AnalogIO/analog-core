using System.Collections.Generic;
using System.Linq;
using CoffeeCard.Common.Models;
using CoffeeCard.Library.Persistence;

namespace CoffeeCard.Library.Services
{
    public class ProgrammeService : IProgrammeService
    {
        private readonly CoffeeCardContext _context;

        public ProgrammeService(CoffeeCardContext context)
        {
            _context = context;
        }

        public IEnumerable<Programme> GetProgrammes()
        {
            return _context.Programmes.AsEnumerable();
        }

        public Programme GetProgramme(int id)
        {
            return _context.Programmes.FirstOrDefault(x => x.Id == id);
        }
    }
}