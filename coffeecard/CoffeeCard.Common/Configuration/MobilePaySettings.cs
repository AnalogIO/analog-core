using System;
using System.ComponentModel.DataAnnotations;
using NetEscapades.Configuration.Validation;

namespace CoffeeCard.Common.Configuration
{
    public class MobilePaySettings : IValidatable
    {
        [Required]
        public Uri ApiUrl { get; set; }

        [Required]
        public Guid ClientId { get; set; }

        [Required]
        public string ClientSecret { get; set; }

        [Required]
        public string MerchantSerialNumber { get; set; }

        [Required]
        public string OcpApimSubscriptionKey { get; set; }

        [Required]
        public string WebhookUrl { get; set; }

        [Required]
        public string AnalogAppRedirectUri { get; set; }

        public void Validate()
        {
            Validator.ValidateObject(this, new ValidationContext(this), true);
        }
    }
}
