using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using MimeKit;
using RestSharp;
using RestSharp.Authenticators;
using Serilog;

namespace CoffeeCard.Library.Services;

public class MailgunEmailSender(MailgunSettings mailgunSettings) : IEmailSender
{
    public async Task SendEmailAsync(MimeMessage mail)
    {
        var options = new RestClientOptions(mailgunSettings.MailgunApiUrl)
        {
            Authenticator = new HttpBasicAuthenticator("api", mailgunSettings.ApiKey),
        };
        using var client = new RestClient(options);

        var request = new RestRequest();
        request.AddParameter("domain", mailgunSettings.Domain, ParameterType.UrlSegment);
        request.Resource = "{domain}/messages";
        request.AddParameter("from", "Cafe Analog <mailgun@cafeanalog.dk>");
        request.AddParameter("to", mail.To[0].ToString());
        request.AddParameter("subject", mail.Subject);
        request.AddParameter("html", mail.HtmlBody);
        request.Method = Method.Post;

        var response = await client.ExecutePostAsync(request);

        if (!response.IsSuccessful)
        {
            Log.Error(
                "Error sending request to Mailgun. StatusCode: {statusCode} ErrorMessage: {errorMessage}",
                response.StatusCode,
                response.ErrorMessage
            );
        }
    }
}

