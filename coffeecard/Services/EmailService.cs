using System;
using System.IO;
using System.Text;
using Coffeecard.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using MailKit.Net.Smtp;

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

        public MimeMessage CreateVerificationEmail(User user, string token)
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
            builder.HtmlBody = builder.HtmlBody.Replace("{baseUrl}", _configuration["baseUrl"]);

            message.From.Add(new MailboxAddress("Café Analog", _configuration["EmailUsername"]));
            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Verify your Café Analog account";

            message.Body = builder.ToMessageBody();

            return message;
        }

        public MimeMessage CreateVerificationEmailForChangedPw(User user, string token, string newEmail)
        {
            throw new NotImplementedException();
        }

        public MimeMessage CreateVerificationEmailForLostPw(User user, string token)
        {
            throw new NotImplementedException();
        }

        public MimeMessage CreateVerificationEmailForRecover(User user, int newPassword)
        {
            throw new NotImplementedException();
        }

        public void SendEmail(MimeMessage mail)
        {
            try
            {
                var client = new SmtpClient();

                client.Connect(_configuration["EmailHost"], int.Parse(_configuration["EmailPort"]), true);
                client.Authenticate(_configuration["EmailUsername"], _configuration["EmailPassword"]);
                client.Send(mail);
                client.Disconnect(true);
                //Console.WriteLine("Send Mail Success.");
            }
            catch (Exception e)
            {
                //Console.WriteLine("Send Mail Failed : " + e.Message);
            }
        }
    }
}
