using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.Ticket;

namespace CoffeeCard.WebApi.Hubs;

public interface IUsedTicketsClient
{
    Task UsedTicket(UsedTicketEvent usedTicketEvent);
}