using System.ComponentModel.DataAnnotations;
using NetEscapades.Configuration.Validation;

namespace CoffeeCard.Common.Configuration
{
    public class MailgunSettings : IValidatable
    {
        [Required]
        public string ApiKey { get; set; }

        [Required]
        public string Domain { get; set; }

        [Required]
        public string EmailBaseUrl { get; set; }

        [Required]
        public string MailgunApiUrl { get; set; }

        public void Validate()
        {
            Validator.ValidateObject(this, new ValidationContext(this), true);
        }
    }
}
