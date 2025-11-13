using System;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace CoffeeCard.Library.Services;

public class SmtpEmailSender(SmtpSettings smtpSettings, ILogger<SmtpEmailSender> logger)
    : IEmailSender
{
    private readonly ILogger<SmtpEmailSender> _logger = logger;

    public async Task SendEmailAsync(MimeMessage mail)
    {
        mail.From.Add(new MailboxAddress("Cafe Analog", "smtp@analogio.dk"));

        try
        {
            using var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync(smtpSettings.Host, smtpSettings.Port);
            await smtpClient.SendAsync(mail);
            await smtpClient.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Error sending request to SMTP server. Error: {errorMessage}",
                ex.Message
            );
        }
    }
}
