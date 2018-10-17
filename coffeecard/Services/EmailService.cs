using System;
using System.IO;
using Coffeecard.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using RestSharp;
using RestSharp.Authenticators;
using System.Text;

namespace coffeecard.Services
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;
        public EmailService(IConfiguration configuration, IHostingEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public void SendRegistrationVerificationEmail(User user, string token)
        {
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
            builder.HtmlBody = builder.HtmlBody.Replace("{baseUrl}", _configuration["EmailBaseUrl"]);

            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Verify your Café Analog account";

            message.Body = builder.ToMessageBody();

            SendEmail(message);
        }

        public void SendVerificationEmailForChangedPw(User user, string token, string newEmail)
        {
            throw new NotImplementedException();
        }

        public void SendVerificationEmailForLostPw(User user, string token)
        {
            throw new NotImplementedException();
        }

        public void SendVerificationEmailForRecover(User user, int newPassword)
        {
            throw new NotImplementedException();
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
