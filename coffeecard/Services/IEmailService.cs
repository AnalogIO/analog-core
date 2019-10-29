using Coffeecard.Models;
using coffeecard.Models.DataTransferObjects.Purchase;
using coffeecard.Models.DataTransferObjects.User;

namespace coffeecard.Services
{
    public interface IEmailService
    {
        void SendInvoice(UserDTO user, PurchaseDTO purchase);
        void SendVerificationEmailForChangedEmail(User user, string token, string newEmail);
        void SendRegistrationVerificationEmail(User user, string token);
        void SendVerificationEmailForLostPw(User user, string token);
        void SendVerificationEmailForRecover(User user, int newPassword);
    }
}
