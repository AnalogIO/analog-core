using System.Collections.Generic;

namespace CoffeeCard.Models.DataTransferObjects.Ticket
{
    public class UseMultipleTicketDTO
    {
        public IEnumerable<int> ProductIds { get; set; }
    }
}