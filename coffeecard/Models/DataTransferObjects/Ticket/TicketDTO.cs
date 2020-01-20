using System;

namespace CoffeeCard.Models.DataTransferObjects.Ticket
{
    public class TicketDTO
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUsed { get; set; }
        public String ProductName { get; set; }
    }
}
