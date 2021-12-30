using System.Collections.Generic;

namespace CoffeeCard.Common.Models.DataTransferObjects.Ticket
{
    public class UseMultipleTicketDto
    {
        public IEnumerable<int> ProductIds { get; set; }
    }
}