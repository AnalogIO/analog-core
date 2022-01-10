using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.Purchase;
using CoffeeCard.Models.DataTransferObjects.User;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services
{
    public interface IEmailService
    {
        Task SendInvoiceAsync(UserDto user, PurchaseDto purchase);
        Task SendVerificationEmailForChangedEmail(User user, string token, string newEmail);
        Task SendRegistrationVerificationEmailAsync(User user, string token);
        Task SendVerificationEmailForLostPwAsync(User user, string token);
    }
}