using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Common.Configuration
{
    public class SmtpSettings
    {
        [Required]
        public string Host { get; set; }

        [Required]
        public int Port { get; set; }
    }
}
