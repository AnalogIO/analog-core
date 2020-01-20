using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using MimeKit;
using RestSharp;
using RestSharp.Authenticators;
using Serilog;
using System;
using System.IO;
using CoffeeCard.Models;

namespace CoffeeCard.Services
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailService(IConfiguration configuration, IHostingEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public void SendRegistrationVerificationEmail(User user, string token)
        {
            Log.Information($"Sending registration verification email to {user.Email} ({user.Id} with token: {token})");
            var fullPath = _httpContextAccessor.HttpContext?.Request?.GetDisplayUrl();
            var baseUrl = fullPath.Substring(0, fullPath.IndexOf("api/"));

            var pathToTemplate = _env.WebRootPath
                            + Path.DirectorySeparatorChar.ToString()
                            + "Templates"
                            + Path.DirectorySeparatorChar.ToString()
                            + "EmailTemplate"
                            + Path.DirectorySeparatorChar.ToString()
                            + "email_verify_registration.html";

            var message = new MimeMessage();
            var builder = new BodyBuilder();

            using (StreamReader SourceReader = File.OpenText(pathToTemplate))
            {
                builder.HtmlBody = SourceReader.ReadToEnd();
            }

            builder.HtmlBody = builder.HtmlBody.Replace("{token}", token);
            builder.HtmlBody = builder.HtmlBody.Replace("{email}", user.Email);
            builder.HtmlBody = builder.HtmlBody.Replace("{name}", user.Name);
            builder.HtmlBody = builder.HtmlBody.Replace("{expiry}", "24 hours");
            builder.HtmlBody = builder.HtmlBody.Replace("{baseUrl}", baseUrl);

            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Verify your Café Analog account";

            message.Body = builder.ToMessageBody();

            SendEmail(message);
        }

        public void SendVerificationEmailForChangedEmail(User user, string token, string newEmail)
        {
            var fullPath = _httpContextAccessor.HttpContext?.Request?.GetDisplayUrl();
            var baseUrl = fullPath.Substring(0, fullPath.IndexOf("api/"));

            var pathToTemplate = _env.WebRootPath
                            + Path.DirectorySeparatorChar.ToString()
                            + "Templates"
                            + Path.DirectorySeparatorChar.ToString()
                            + "EmailTemplate"
                            + Path.DirectorySeparatorChar.ToString()
                            + "email_verify_updatedemail.html";

            var message = new MimeMessage();
            var builder = new BodyBuilder();

            using (StreamReader SourceReader = File.OpenText(pathToTemplate))
            {
                builder.HtmlBody = SourceReader.ReadToEnd();
            }

            builder.HtmlBody = builder.HtmlBody.Replace("{token}", token);
            builder.HtmlBody = builder.HtmlBody.Replace("{email}", user.Email);
            builder.HtmlBody = builder.HtmlBody.Replace("{newEmail}", newEmail);
            builder.HtmlBody = builder.HtmlBody.Replace("{name}", user.Name);
            builder.HtmlBody = builder.HtmlBody.Replace("{expiry}", "24 hours");
            builder.HtmlBody = builder.HtmlBody.Replace("{baseUrl}", baseUrl);

            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Verify your new email for your Café Analog account";

            message.Body = builder.ToMessageBody();

            SendEmail(message);
        }

        public void SendVerificationEmailForLostPw(User user, string token)
        {
            var fullPath = _httpContextAccessor.HttpContext?.Request?.GetDisplayUrl();
            var baseUrl = fullPath.Substring(0, fullPath.IndexOf("api/"));

            var pathToTemplate = _env.WebRootPath
                            + Path.DirectorySeparatorChar.ToString()
                            + "Templates"
                            + Path.DirectorySeparatorChar.ToString()
                            + "EmailTemplate"
                            + Path.DirectorySeparatorChar.ToString()
                            + "email_verify_lostpassword.html";

            var message = new MimeMessage();
            var builder = new BodyBuilder();

            using (StreamReader SourceReader = File.OpenText(pathToTemplate))
            {
                builder.HtmlBody = SourceReader.ReadToEnd();
            }

            builder.HtmlBody = builder.HtmlBody.Replace("{token}", token);
            builder.HtmlBody = builder.HtmlBody.Replace("{email}", user.Email);
            builder.HtmlBody = builder.HtmlBody.Replace("{name}", user.Name);
            builder.HtmlBody = builder.HtmlBody.Replace("{expiry}", "24 hours");
            builder.HtmlBody = builder.HtmlBody.Replace("{baseUrl}", baseUrl);

            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Café Analog account lost PIN request";

            message.Body = builder.ToMessageBody();

            SendEmail(message);
        }

        public void SendVerificationEmailForRecover(User user, int newPassword)
        {
            Log.Information($"Sending email to {user.Email} ");
            var fullPath = _httpContextAccessor.HttpContext?.Request?.GetDisplayUrl();
            var baseUrl = fullPath.Substring(0, fullPath.IndexOf("api/"));

            var pathToTemplate = _env.WebRootPath
                            + Path.DirectorySeparatorChar.ToString()
                            + "Templates"
                            + Path.DirectorySeparatorChar.ToString()
                            + "EmailTemplate"
                            + Path.DirectorySeparatorChar.ToString()
                            + "email_newpassword.html";

            var message = new MimeMessage();
            var builder = new BodyBuilder();

            using (StreamReader SourceReader = File.OpenText(pathToTemplate))
            {
                builder.HtmlBody = SourceReader.ReadToEnd();
            }

            builder.HtmlBody = builder.HtmlBody.Replace("{password}", newPassword.ToString());
            builder.HtmlBody = builder.HtmlBody.Replace("{email}", user.Email);
            builder.HtmlBody = builder.HtmlBody.Replace("{name}", user.Name);

            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Your new PIN for your Café Analog account";

            message.Body = builder.ToMessageBody();

            SendEmail(message);
        }

        public void SendEmail(MimeMessage mail)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");

            client.Authenticator = new HttpBasicAuthenticator("api", _configuration["MailgunAPIKey"]);
            RestRequest request = new RestRequest();
            request.AddParameter("domain", _configuration["MailgunDomain"], ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Café Analog <mailgun@cafeanalog.dk>");
            request.AddParameter("to", mail.To[0]);
            request.AddParameter("subject", mail.Subject);
            request.AddParameter("html", mail.HtmlBody);
            request.AddParameter("text", mail.TextBody);
            request.Method = Method.POST;
            var response = client.Execute(request);
            Console.WriteLine(response.IsSuccessful);
        }
    }
}
