using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeecard.Services
{
    public interface IHashService
    {
        string GenerateSalt();
        string Hash(string password);
    }
}
