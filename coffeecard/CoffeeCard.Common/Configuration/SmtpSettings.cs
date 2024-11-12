using System.ComponentModel.DataAnnotations;
using NetEscapades.Configuration.Validation;

namespace CoffeeCard.Common.Configuration
{
    public class SmtpSettings
    {
        [Required] public string Host { get; set; }
        [Required] public int Port { get; set; }
    }
}

