using System.ComponentModel.DataAnnotations;
using NetEscapades.Configuration.Validation;

namespace CoffeeCard.Common.Configuration
{
    public class MobilePaySettings : IValidatable
    {
        [Required] public string MerchantId { get; set; }

        [Required] public string SubscriptionKey { get; set; }

        [Required] public string CertificateName { get; set; }

        [Required] public string CertificatePassword { get; set; }

        public void Validate()
        {
            Validator.ValidateObject(this, new ValidationContext(this), true);
        }
    }
}