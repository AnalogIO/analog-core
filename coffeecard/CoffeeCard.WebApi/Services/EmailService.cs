﻿using System;
using System.IO;
using CoffeeCard.Common.Configuration;
using CoffeeCard.WebApi.Models;
using CoffeeCard.WebApi.Models.DataTransferObjects.Purchase;
using CoffeeCard.WebApi.Models.DataTransferObjects.User;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using RestSharp;
using RestSharp.Authenticators;
using Serilog;

namespace CoffeeCard.WebApi.Services
{
    public class EmailService : IEmailService
    {
        private const string MailgunApiUrl = "https://api.mailgun.net/v3";
        private readonly IWebHostEnvironment _env;
        private readonly EnvironmentSettings _environmentSettings;

        private readonly MailgunSettings _mailgunSettings;

        public EmailService(MailgunSettings mailgunSettings, EnvironmentSettings environmentSettings,
            IWebHostEnvironment env)
        {
            _mailgunSettings = mailgunSettings;
            _environmentSettings = environmentSettings;
            _env = env;
        }

        public void SendInvoice(UserDTO user, PurchaseDTO purchase)
        {
            var message = new MimeMessage();
            var builder = RetrieveTemplate("invoice.html").Item1;
            var utcTime = DateTime.UtcNow;
            var cetTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utcTime, "Central Europe Standard Time");

            builder.HtmlBody = builder.HtmlBody.Replace("{email}", user.Email);
            builder.HtmlBody = builder.HtmlBody.Replace("{name}", user.Name);
            builder.HtmlBody = builder.HtmlBody.Replace("{quantity}", purchase.NumberOfTickets.ToString());
            builder.HtmlBody = builder.HtmlBody.Replace("{product}", purchase.ProductName);
            builder.HtmlBody = builder.HtmlBody.Replace("{vat}", (purchase.Price * 0.2).ToString());
            builder.HtmlBody = builder.HtmlBody.Replace("{price}", purchase.Price.ToString());
            builder.HtmlBody = builder.HtmlBody.Replace("{orderId}", purchase.OrderId);
            builder.HtmlBody = builder.HtmlBody.Replace("{date}", cetTime.ToShortDateString());

            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Thank you for your purchase at Cafe Analog";

            message.Body = builder.ToMessageBody();

            SendEmail(message);
        }

        public void SendRegistrationVerificationEmail(User user, string token)
        {
            Log.Information($"Sending registration verification email to {user.Email} ({user.Id})");
            var message = new MimeMessage();
            var builderAndBaseUrl = RetrieveTemplate("email_verify_registration.html");
            var builder = builderAndBaseUrl.Item1;
            var baseUrl = builderAndBaseUrl.Item2;

            builder.HtmlBody = builder.HtmlBody.Replace("{token}", token);
            builder.HtmlBody = builder.HtmlBody.Replace("{email}", user.Email);
            builder.HtmlBody = builder.HtmlBody.Replace("{name}", user.Name);
            builder.HtmlBody = builder.HtmlBody.Replace("{expiry}", "24 hours");
            builder.HtmlBody = builder.HtmlBody.Replace("{baseUrl}", baseUrl);

            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Verify your Cafe Analog account";

            message.Body = builder.ToMessageBody();

            SendEmail(message);
        }

        public void SendVerificationEmailForChangedEmail(User user, string token, string newEmail)
        {
            var message = new MimeMessage();
            var builderAndBaseUrl = RetrieveTemplate("email_verify_updatedemail.html");
            var builder = builderAndBaseUrl.Item1;
            var baseUrl = builderAndBaseUrl.Item2;

            builder.HtmlBody = builder.HtmlBody.Replace("{token}", token);
            builder.HtmlBody = builder.HtmlBody.Replace("{email}", user.Email);
            builder.HtmlBody = builder.HtmlBody.Replace("{newEmail}", newEmail);
            builder.HtmlBody = builder.HtmlBody.Replace("{name}", user.Name);
            builder.HtmlBody = builder.HtmlBody.Replace("{expiry}", "24 hours");
            builder.HtmlBody = builder.HtmlBody.Replace("{baseUrl}", baseUrl);

            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Verify your new email for your Cafe Analog account";

            message.Body = builder.ToMessageBody();

            SendEmail(message);
        }

        public void SendVerificationEmailForLostPw(User user, string token)
        {
            var message = new MimeMessage();
            var builderAndBaseUrl = RetrieveTemplate("email_verify_lostpassword.html");
            var builder = builderAndBaseUrl.Item1;
            var baseUrl = builderAndBaseUrl.Item2;

            builder.HtmlBody = builder.HtmlBody.Replace("{token}", token);
            builder.HtmlBody = builder.HtmlBody.Replace("{email}", user.Email);
            builder.HtmlBody = builder.HtmlBody.Replace("{name}", user.Name);
            builder.HtmlBody = builder.HtmlBody.Replace("{expiry}", "24 hours");
            builder.HtmlBody = builder.HtmlBody.Replace("{baseUrl}", baseUrl);

            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Cafe Analog account lost PIN request";

            message.Body = builder.ToMessageBody();

            SendEmail(message);
        }

        private (BodyBuilder, string) RetrieveTemplate(string templateName)
        {
            var baseUrl = _environmentSettings.DeploymentUrl;

            var pathToTemplate = _env.WebRootPath
                                 + Path.DirectorySeparatorChar
                                 + "Templates"
                                 + Path.DirectorySeparatorChar
                                 + "EmailTemplate"
                                 + Path.DirectorySeparatorChar
                                 + templateName;

            var builder = new BodyBuilder();

            using (var sourceReader = File.OpenText(pathToTemplate))
            {
                builder.HtmlBody = sourceReader.ReadToEnd();
            }

            return (builder, baseUrl);
        }

        private void SendEmail(MimeMessage mail)
        {
            var client = new RestClient
            {
                BaseUrl = new Uri(MailgunApiUrl),
                Authenticator = new HttpBasicAuthenticator("api", _mailgunSettings.ApiKey)
            };

            var request = new RestRequest();
            request.AddParameter("domain", _mailgunSettings.Domain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Cafe Analog <mailgun@cafeanalog.dk>");
            request.AddParameter("to", mail.To[0]);
            request.AddParameter("subject", mail.Subject);
            request.AddParameter("html", mail.HtmlBody);
            request.AddParameter("text", mail.TextBody);
            request.Method = Method.POST;
            client.Execute(request);
        }
    }
}