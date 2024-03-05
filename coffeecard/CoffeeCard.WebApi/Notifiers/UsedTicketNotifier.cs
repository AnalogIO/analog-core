using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.Ticket;
using CoffeeCard.WebApi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CoffeeCard.WebApi.Notifiers;

public class UsedTicketNotifier : IUsedTicketNotifier
{
    private readonly IHubContext<UsedTicketsHub, IUsedTicketsClient> _hubContext;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="hubContext"></param>
    public UsedTicketNotifier(IHubContext<UsedTicketsHub, IUsedTicketsClient> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>
    /// Notifies all clients of a swipe
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="usedTicketResponse"></param>
    /// <returns></returns>
    public Task NotifyClientsOfSwipe(string userName, UsedTicketResponse usedTicketResponse)
    {
        return _hubContext.Clients.All.UsedTicket(new UsedTicketEvent
        {
            UserName = userName,
            ProductName = usedTicketResponse.ProductName,
            DateUsed = usedTicketResponse.DateUsed,
            MenuItemName = usedTicketResponse.MenuItemName
        });
    }
}