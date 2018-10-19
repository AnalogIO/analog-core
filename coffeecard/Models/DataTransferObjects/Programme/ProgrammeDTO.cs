using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeecard.Models.DataTransferObjects.Programme
{
    public class ProgrammeDTO
    {
        public int Id { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
    }
}
