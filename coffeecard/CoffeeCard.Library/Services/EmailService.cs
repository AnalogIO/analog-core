using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Models.DataTransferObjects.Purchase;
using CoffeeCard.Models.DataTransferObjects.User;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using TimeZoneConverter;

namespace CoffeeCard.Library.Services
{
    public class EmailService : IEmailService
    {
        private readonly IWebHostEnvironment _env;
        private readonly EnvironmentSettings _environmentSettings;
        private readonly IEmailSender _emailSender;
        private readonly IMapperService _mapperService;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IEmailSender emailSender,
            EnvironmentSettings environmentSettings,
            IWebHostEnvironment env,
            IMapperService mapperService,
            ILogger<EmailService> logger
        )
        {
            _emailSender = emailSender;
            _environmentSettings = environmentSettings;
            _env = env;
            _mapperService = mapperService;
            _logger = logger;
        }

        public async Task SendInvoiceAsync(UserDto user, PurchaseDto purchase)
        {
            var message = new MimeMessage();
            var builder = RetrieveTemplate("invoice.html");
            var utcTime = DateTime.UtcNow;
            var dkTimeZone = TZConvert.GetTimeZoneInfo("Europe/Copenhagen");
            var dkTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, dkTimeZone);

            builder.HtmlBody = builder.HtmlBody.Replace("{email}", user.Email);
            builder.HtmlBody = builder.HtmlBody.Replace("{name}", user.Name);
            builder.HtmlBody = builder.HtmlBody.Replace(
                "{quantity}",
                purchase.NumberOfTickets.ToString()
            );
            builder.HtmlBody = builder.HtmlBody.Replace("{product}", purchase.ProductName);
            builder.HtmlBody = builder.HtmlBody.Replace(
                "{vat}",
                (purchase.Price * 0.2).ToString(CultureInfo.InvariantCulture)
            );
            builder.HtmlBody = builder.HtmlBody.Replace("{price}", purchase.Price.ToString());
            builder.HtmlBody = builder.HtmlBody.Replace("{orderId}", purchase.OrderId);
            builder.HtmlBody = builder.HtmlBody.Replace("{date}", dkTime.ToShortDateString());

            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Thank you for your purchase at Cafe Analog";

            message.Body = builder.ToMessageBody();

            _logger.LogInformation(
                "Sending invoice for PurchaseId {PurchaseId} to UserId {UserId}, E-mail {Email}",
                purchase.Id,
                user.Id,
                user.Email
            );

            await _emailSender.SendEmailAsync(message);
        }

        public async Task SendRegistrationVerificationEmailAsync(User user, string token)
        {
            _logger.LogInformation(
                "Sending registration verification email to {email} {userid}",
                user.Email,
                user.Id
            );
            var message = new MimeMessage();
            var builder = RetrieveTemplate("email_verify_registration.html");
            const string endpoint = "verifyemail?token=";

            builder = BuildVerifyEmail(builder, token, user.Email, user.Name, endpoint);

            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Verify your Cafe Analog account";

            message.Body = builder.ToMessageBody();

            await _emailSender.SendEmailAsync(message);
        }

        public async Task SendVerificationEmailForLostPwAsync(User user, string token)
        {
            var message = new MimeMessage();
            var builder = RetrieveTemplate("email_verify_lostpassword.html");
            const string endpoint = "recover?token=";

            builder = BuildVerifyEmail(builder, token, user.Email, user.Name, endpoint);

            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Cafe Analog account lost PIN request";

            message.Body = builder.ToMessageBody();

            await _emailSender.SendEmailAsync(message);
        }

        public async Task SendVerificationEmailForDeleteAccount(User user, string token)
        {
            _logger.LogInformation(
                "Sending delete verification email to {email} {userid}",
                user.Email,
                user.Id
            );
            var message = new MimeMessage();
            var builder = RetrieveTemplate("email_verify_account_deletion.html");
            const string endpoint = "verifydelete?token=";

            builder = BuildVerifyEmail(builder, token, user.Email, user.Name, endpoint);

            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Verify the deletion of your Cafe Analog account";

            message.Body = builder.ToMessageBody();

            await _emailSender.SendEmailAsync(message);
        }

        public async Task SendInvoiceAsyncV2(Purchase purchase, User user)
        {
            var purchaseDto = _mapperService.Map(purchase);
            var userDto = _mapperService.Map(user);

            await SendInvoiceAsync(userDto, purchaseDto);
        }

        private BodyBuilder BuildVerifyEmail(
            BodyBuilder builder,
            string token,
            string email,
            string name,
            string endpoint
        )
        {
            var baseUrl = _environmentSettings.DeploymentUrl;

            builder.HtmlBody = builder.HtmlBody.Replace("{email}", email);
            builder.HtmlBody = builder.HtmlBody.Replace("{name}", name);
            builder.HtmlBody = builder.HtmlBody.Replace("{expiry}", "24 hours");
            builder.HtmlBody = builder.HtmlBody.Replace("{baseUrl}", baseUrl);
            builder.HtmlBody = builder.HtmlBody.Replace("{endpoint}", endpoint);
            builder.HtmlBody = builder.HtmlBody.Replace("{token}", token);

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
