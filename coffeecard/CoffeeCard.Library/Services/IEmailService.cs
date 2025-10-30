using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.Purchase;
using CoffeeCard.Models.DataTransferObjects.User;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services
{
    public interface IEmailService
    {
        Task SendInvoiceAsync(UserDto user, PurchaseDto purchase);
        Task SendRegistrationVerificationEmailAsync(User user, string token);
        Task SendVerificationEmailForLostPwAsync(User user, string token);
        Task SendVerificationEmailForDeleteAccount(User user, string token);
        Task SendInvoiceAsyncV2(Purchase purchase, User user);
    }
}
