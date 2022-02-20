using System.Threading.Tasks;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2
{
    public interface ITicketService
    {
        Task IssueTickets(Purchase purchase);
    }
}