using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeecard.Models.DataTransferObjects.Ticket
{
    public class TicketDTO
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUsed { get; set; }
        public String ProductName { get; set; }
    }
}
