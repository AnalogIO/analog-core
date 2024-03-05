using System;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.Ticket;

namespace CoffeeCard.WebApi.Notifiers;

public interface IUsedTicketNotifier
{
    Task NotifyClientsOfSwipe(string userName, UsedTicketResponse usedTicketResponse);
}