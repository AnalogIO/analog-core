using CoffeeCard.Common.Models;
using CoffeeCard.Common.Models.DataTransferObjects.Purchase;
using CoffeeCard.Common.Models.DataTransferObjects.User;

namespace CoffeeCard.Library.Services
{
    public interface IEmailService
    {
        void SendInvoice(UserDto user, PurchaseDto purchase);
        void SendVerificationEmailForChangedEmail(User user, string token, string newEmail);
        void SendRegistrationVerificationEmail(User user, string token);
        void SendVerificationEmailForLostPw(User user, string token);
    }
}