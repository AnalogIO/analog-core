using Coffeecard.Models;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace coffeecard.Services
{
    public interface IEmailService
    {
        void SendEmail(MimeMessage mail);
        void SendVerificationEmailForChangedPw(User user, string token, string newEmail);
        void SendRegistrationVerificationEmail(User user, string token);
        void SendVerificationEmailForLostPw(User user, string token);
        void SendVerificationEmailForRecover(User user, int newPassword);
    }
}
