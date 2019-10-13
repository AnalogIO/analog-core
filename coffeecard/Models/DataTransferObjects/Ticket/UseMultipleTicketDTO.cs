using System.Collections.Generic;

namespace coffeecard.Models.DataTransferObjects.Ticket
{
    public class UseMultipleTicketDTO
    {
        public IEnumerable<int> ProductIds { get; set; }
    }
}
