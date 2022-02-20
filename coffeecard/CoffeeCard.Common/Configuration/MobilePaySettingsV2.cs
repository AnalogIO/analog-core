using System;
using System.ComponentModel.DataAnnotations;
using NetEscapades.Configuration.Validation;

namespace CoffeeCard.Common.Configuration
{
    public class MobilePaySettingsV2  : IValidatable
    {
        [Required]
        public Uri ApiUrl { get; set; }
        
        [Required]
        public string ApiKey { get; set; }
        
        [Required]
        public string ClientId { get; set; }
        
        [Required]
        public Guid PaymentPointId { get; set; }

        public void Validate()
        {
            Validator.ValidateObject(this, new ValidationContext(this), true);
        }
    }
}