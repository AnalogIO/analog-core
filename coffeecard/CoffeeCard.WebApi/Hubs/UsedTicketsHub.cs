using System;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.Ticket;
using Microsoft.AspNetCore.SignalR;

namespace CoffeeCard.WebApi.Hubs;

public class UsedTicketsHub : Hub<IUsedTicketsClient>
{
}