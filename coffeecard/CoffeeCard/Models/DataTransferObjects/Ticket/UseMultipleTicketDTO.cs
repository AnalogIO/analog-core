using System.Collections.Generic;

namespace CoffeeCard.WebApi.Models.DataTransferObjects.Ticket
{
    public class UseMultipleTicketDTO
    {
        public IEnumerable<int> ProductIds { get; set; }
    }
}