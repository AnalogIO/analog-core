using System.Threading.Tasks;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2
{
    public interface IEmailService
    {
        Task SendMagicLink(User user, string magicLink, LoginType loginType);

        /// <summary>
        /// Sends a welcome email containing a single-use link that lets the
        /// owner of <paramref name="email"/> complete their registration.
        /// </summary>
        Task SendRegistrationLink(string email, string registrationToken, LoginType loginType);
    }
}
