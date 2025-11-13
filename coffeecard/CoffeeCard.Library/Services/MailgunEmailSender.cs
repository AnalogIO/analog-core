using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using RestSharp;

namespace CoffeeCard.Library.Services;

public class MailgunEmailSender(
    MailgunSettings mailgunSettings,
    IRestClient restClient,
    ILogger<MailgunEmailSender> logger
) : IEmailSender
{
    private readonly ILogger<MailgunEmailSender> _logger = logger;

    public async Task SendEmailAsync(MimeMessage mail)
    {
        var request = new RestRequest();
        request.AddParameter("domain", mailgunSettings.Domain, ParameterType.UrlSegment);
        request.Resource = "{domain}/messages";
        request.AddParameter("from", "Cafe Analog <mailgun@cafeanalog.dk>");
        request.AddParameter("to", mail.To[0].ToString());
        request.AddParameter("subject", mail.Subject);
        request.AddParameter("html", mail.HtmlBody);
        request.Method = Method.Post;

        var response = await restClient.ExecutePostAsync(request);

        if (!response.IsSuccessful)
        {
            _logger.LogError(
                "Error sending request to Mailgun. StatusCode: {statusCode} ErrorMessage: {errorMessage}",
                response.StatusCode,
                response.ErrorMessage
            );
        }
    }
}
