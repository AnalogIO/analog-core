using System;

namespace CoffeeCard.WebApi.Models.DataTransferObjects.Ticket
{
    public class TicketDTO
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUsed { get; set; }
        public string ProductName { get; set; }
    }
}