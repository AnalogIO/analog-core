using System;
using System.IO;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace CoffeeCard.Library.Services.v2
{
    public class EmailService : IEmailService
    {
        private readonly IWebHostEnvironment _env;
        private readonly EnvironmentSettings _environmentSettings;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IEmailSender emailSender,
            EnvironmentSettings environmentSettings,
            IWebHostEnvironment env,
            ILogger<EmailService> logger
        )
        {
            _emailSender = emailSender;
            _environmentSettings = environmentSettings;
            _env = env;
            _logger = logger;
        }

        public async Task SendMagicLink(User user, string magicLink, LoginType loginType)
        {
            _logger.LogInformation(
                "Sending magic link email to {email} {userid}",
                user.Email,
                user.Id
            );
            var message = new MimeMessage();
            var builder = RetrieveTemplate("email_magic_link_login.html");
            var baseUrl = loginType switch
            {
                LoginType.Shifty => _environmentSettings.ShiftyUrl,
                LoginType.App => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };

            var deeplink = loginType.GetDeepLink(baseUrl, magicLink);

            builder = BuildMagicLinkEmail(builder, user.Email, user.Name, deeplink);

            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Login to Analog";

            message.Body = builder.ToMessageBody();

            await _emailSender.SendEmailAsync(message);
        }

        private static BodyBuilder BuildMagicLinkEmail(
            BodyBuilder builder,
            string email,
            string name,
            string deeplink
        )
        {
            builder.HtmlBody = builder.HtmlBody.Replace("{email}", email);
            builder.HtmlBody = builder.HtmlBody.Replace("{name}", name);
            builder.HtmlBody = builder.HtmlBody.Replace("{expires}", "30 minutes");
            builder.HtmlBody = builder.HtmlBody.Replace("{deeplink}", deeplink);

            return builder;
        }

        private BodyBuilder RetrieveTemplate(string templateName)
        {
            var pathToTemplate =
                _env.WebRootPath
                + Path.DirectorySeparatorChar
                + "Templates"
                + Path.DirectorySeparatorChar
                + "EmailTemplate"
                + Path.DirectorySeparatorChar
                + "GeneratedEmails"
                + Path.DirectorySeparatorChar
                + templateName;

            var builder = new BodyBuilder();

            using (var sourceReader = File.OpenText(pathToTemplate))
            {
                builder.HtmlBody = sourceReader.ReadToEnd();
            }

            return builder;
        }
    }
}
