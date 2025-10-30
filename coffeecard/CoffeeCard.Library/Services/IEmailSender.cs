using System.Threading.Tasks;
using MimeKit;

namespace CoffeeCard.Library.Services;

public interface IEmailSender
{
    public Task SendEmailAsync(MimeMessage email);
}
