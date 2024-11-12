﻿using System;
using System.IO;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Models.DataTransferObjects.Purchase;
using CoffeeCard.Models.DataTransferObjects.User;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using RestSharp;
using RestSharp.Authenticators;
using Serilog;
using TimeZoneConverter;

namespace CoffeeCard.Library.Services.v2
{
    public class EmailService : IEmailService
    {
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

        public async Task SendMagicLink(User user, string magicLink, LoginType loginType)
        {
            Log.Information("Sending magic link email to {email} {userid}", user.Email, user.Id);
            var message = new MimeMessage();
            var builder = RetrieveTemplate("email_magic_link_login.html");
            var baseUrl = loginType switch
            {
                LoginType.Shifty => _environmentSettings.ShiftyUrl,
                LoginType.App => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };

            var deeplink = loginType.GetDeepLink(baseUrl, magicLink);

            Console.WriteLine($"MAGIC LINK HREF: {deeplink}");

            builder = BuildMagicLinkEmail(builder, user.Email, user.Name, deeplink);

            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Login to Analog";

            message.Body = builder.ToMessageBody();

            await SendEmailAsync(message);
        }

        private static BodyBuilder BuildMagicLinkEmail(BodyBuilder builder, string email, string name, string deeplink)
        {
            builder.HtmlBody = builder.HtmlBody.Replace("{email}", email);
            builder.HtmlBody = builder.HtmlBody.Replace("{name}", name);
            builder.HtmlBody = builder.HtmlBody.Replace("{expiry}", "30 minutes");
            builder.HtmlBody = builder.HtmlBody.Replace("{deeplink}", deeplink);

            return builder;
        }

        private BodyBuilder RetrieveTemplate(string templateName)
        {
            var pathToTemplate = _env.WebRootPath
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

        private async Task SendEmailAsync(MimeMessage mail)
        {
            var client = new RestClient(_mailgunSettings.MailgunApiUrl)
            {
                Authenticator = new HttpBasicAuthenticator("api", _mailgunSettings.ApiKey)
            };

            var request = new RestRequest();
            request.AddParameter("domain", _mailgunSettings.Domain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Cafe Analog <mailgun@cafeanalog.dk>");
            request.AddParameter("to", mail.To[0].ToString());
            request.AddParameter("subject", mail.Subject);
            request.AddParameter("html", mail.HtmlBody);
            request.Method = Method.Post;

            var response = await client.ExecutePostAsync(request);

            if (!response.IsSuccessful)
            {
                Log.Error("Error sending request to Mailgun. StatusCode: {statusCode} ErrorMessage: {errorMessage}", response.StatusCode, response.ErrorMessage);
            }
        }
    }
}