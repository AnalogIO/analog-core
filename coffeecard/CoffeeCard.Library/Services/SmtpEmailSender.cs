using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using MailKit.Net.Smtp;
using MimeKit;

namespace CoffeeCard.Library.Services;

public class SmtpEmailSender(SmtpSettings smtpSettings) : IEmailSender
{
    public async Task SendEmailAsync(MimeMessage mail)
    {
        mail.From.Add(new MailboxAddress("Cafe Analog", "smtp@analogio.dk"));

        using var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync(smtpSettings.Host, smtpSettings.Port);
        await smtpClient.SendAsync(mail);
        await smtpClient.DisconnectAsync(true);
    }
}