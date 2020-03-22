using System.Collections.Generic;

namespace CoffeeCard.WebApi.Models.DataTransferObjects.Ticket
{
    public class UseMultipleTicketDto
    {
        public IEnumerable<int> ProductIds { get; set; }
    }
}