using Coffeecard.Models;
using coffeecard.Models.DataTransferObjects.Purchase;
using MimeKit;

namespace coffeecard.Services
{
    public interface IEmailService
    {
        void SendReceipt(User user, PurchaseDTO purchase);
        void SendVerificationEmailForChangedEmail(User user, string token, string newEmail);
        void SendRegistrationVerificationEmail(User user, string token);
        void SendVerificationEmailForLostPw(User user, string token);
        void SendVerificationEmailForRecover(User user, int newPassword);
    }
}
