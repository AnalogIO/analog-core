using CoffeeCard.Common.Configuration;
using CoffeeCard.Models.DataTransferObjects.Purchase;
using CoffeeCard.Models.DataTransferObjects.User;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using RestSharp;
using RestSharp.Authenticators;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace CoffeeCard.Library.Services
{
    public class EmailService : IEmailService
    {
        private readonly IWebHostEnvironment _env;
        private readonly EnvironmentSettings _environmentSettings;
        private readonly MailgunSettings _mailgunSettings;
        private readonly IMapperService _mapperService;

        public EmailService(MailgunSettings mailgunSettings, EnvironmentSettings environmentSettings,
            IWebHostEnvironment env, IMapperService mapperService)
        {
            _mailgunSettings = mailgunSettings;
            _environmentSettings = environmentSettings;
            _env = env;
            _mapperService = mapperService;
        }

        public async Task SendInvoiceAsync(UserDto user, PurchaseDto purchase)
        {
            MimeMessage message = new MimeMessage();
            BodyBuilder builder = RetrieveTemplate("invoice.html");
            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo dkTimeZone = TZConvert.GetTimeZoneInfo("Europe/Copenhagen");
            DateTime dkTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, dkTimeZone);

            builder.HtmlBody = builder.HtmlBody.Replace("{email}", user.Email);
            builder.HtmlBody = builder.HtmlBody.Replace("{name}", user.Name);
            builder.HtmlBody = builder.HtmlBody.Replace("{quantity}", purchase.NumberOfTickets.ToString());
            builder.HtmlBody = builder.HtmlBody.Replace("{product}", purchase.ProductName);
            builder.HtmlBody = builder.HtmlBody.Replace("{vat}", (purchase.Price * 0.2).ToString());
            builder.HtmlBody = builder.HtmlBody.Replace("{price}", purchase.Price.ToString());
            builder.HtmlBody = builder.HtmlBody.Replace("{orderId}", purchase.OrderId);
            builder.HtmlBody = builder.HtmlBody.Replace("{date}", dkTime.ToShortDateString());

            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Thank you for your purchase at Cafe Analog";

            message.Body = builder.ToMessageBody();

            Log.Information("Sending invoice for PurchaseId {PurchaseId} to UserId {UserId}, E-mail {Email}", purchase.Id, user.Id, user.Email);

            await SendEmailAsync(message);
        }

        public async Task SendRegistrationVerificationEmailAsync(User user, string token)
        {
            Log.Information("Sending registration verification email to {email} {userid}", user.Email, user.Id);
            MimeMessage message = new MimeMessage();
            BodyBuilder builder = RetrieveTemplate("email_verify_registration.html");
            const string endpoint = "verifyemail?token=";

            builder = BuildVerifyEmail(builder, token, user.Email, user.Name, endpoint);

            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Verify your Cafe Analog account";

            message.Body = builder.ToMessageBody();

            await SendEmailAsync(message);
        }

        public async Task SendVerificationEmailForLostPwAsync(User user, string token)
        {
            MimeMessage message = new MimeMessage();
            BodyBuilder builder = RetrieveTemplate("email_verify_lostpassword.html");
            const string endpoint = "recover?token=";

            builder = BuildVerifyEmail(builder, token, user.Email, user.Name, endpoint);

            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Cafe Analog account lost PIN request";

            message.Body = builder.ToMessageBody();

            await SendEmailAsync(message);
        }

        public async Task SendVerificationEmailForDeleteAccount(User user, string token)
        {
            Log.Information("Sending delete verification email to {email} {userid}", user.Email, user.Id);
            MimeMessage message = new MimeMessage();
            BodyBuilder builder = RetrieveTemplate("email_verify_account_deletion.html");
            const string endpoint = "verifydelete?token=";

            builder = BuildVerifyEmail(builder, token, user.Email, user.Name, endpoint);

            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Verify the deletion of your Cafe Analog account";

            message.Body = builder.ToMessageBody();

            await SendEmailAsync(message);
        }

        public async Task SendInvoiceAsyncV2(Purchase purchase, User user)
        {
            PurchaseDto purchaseDto = _mapperService.Map(purchase);
            UserDto userDto = _mapperService.Map(user);

            await SendInvoiceAsync(userDto, purchaseDto);
        }

        private BodyBuilder BuildVerifyEmail(BodyBuilder builder, string token, string email, string name, string endpoint)
        {
            string baseUrl = _environmentSettings.DeploymentUrl;

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
            string pathToTemplate = _env.WebRootPath
                                 + Path.DirectorySeparatorChar
                                 + "Templates"
                                 + Path.DirectorySeparatorChar
                                 + "EmailTemplate"
                                 + Path.DirectorySeparatorChar
                                 + "GeneratedEmails"
                                 + Path.DirectorySeparatorChar
                                 + templateName;

            BodyBuilder builder = new BodyBuilder();

            using (StreamReader sourceReader = File.OpenText(pathToTemplate))
            {
                builder.HtmlBody = sourceReader.ReadToEnd();
            }

            return builder;
        }

        private async Task SendEmailAsync(MimeMessage mail)
        {
            RestClient client = new RestClient(_mailgunSettings.MailgunApiUrl)
            {
                Authenticator = new HttpBasicAuthenticator("api", _mailgunSettings.ApiKey)
            };

            RestRequest request = new RestRequest();
            _ = request.AddParameter("domain", _mailgunSettings.Domain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            _ = request.AddParameter("from", "Cafe Analog <mailgun@cafeanalog.dk>");
            _ = request.AddParameter("to", mail.To[0].ToString());
            _ = request.AddParameter("subject", mail.Subject);
            _ = request.AddParameter("html", mail.HtmlBody);
            request.Method = Method.Post;

            RestResponse response = await client.ExecutePostAsync(request);

            if (!response.IsSuccessful)
            {
                Log.Error("Error sending request to Mailgun. StatusCode: {statusCode} ErrorMessage: {errorMessage}", response.StatusCode, response.ErrorMessage);
            }
        }
    }
}