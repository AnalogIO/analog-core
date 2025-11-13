using System.Threading.Tasks;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2
{
    public interface IEmailService
    {
        Task SendMagicLink(User user, string magicLink, LoginType loginType);
    }
}
