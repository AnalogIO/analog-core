using CoffeeCard.WebApi.Models;
using CoffeeCard.WebApi.Models.DataTransferObjects.Purchase;
using CoffeeCard.WebApi.Models.DataTransferObjects.User;

namespace CoffeeCard.WebApi.Services
{
    public interface IEmailService
    {
        void SendInvoice(UserDTO user, PurchaseDTO purchase);
        void SendVerificationEmailForChangedEmail(User user, string token, string newEmail);
        void SendRegistrationVerificationEmail(User user, string token);
        void SendVerificationEmailForLostPw(User user, string token);
    }
}