using CoffeeCard.Models;
using MimeKit;

namespace CoffeeCard.Services
{
    public interface IEmailService
    {
        void SendEmail(MimeMessage mail);
        void SendVerificationEmailForChangedEmail(User user, string token, string newEmail);
        void SendRegistrationVerificationEmail(User user, string token);
        void SendVerificationEmailForLostPw(User user, string token);
        void SendVerificationEmailForRecover(User user, int newPassword);
    }
}