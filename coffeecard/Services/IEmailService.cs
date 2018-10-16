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
        MimeMessage CreateVerificationEmailForChangedPw(User user, string token, string newEmail);
        MimeMessage CreateVerificationEmail(User user, string token);
        MimeMessage CreateVerificationEmailForLostPw(User user, string token);
        MimeMessage CreateVerificationEmailForRecover(User user, int newPassword);
    }
}
