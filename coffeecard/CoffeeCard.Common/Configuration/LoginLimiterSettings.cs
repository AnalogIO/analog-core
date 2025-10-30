using System.ComponentModel.DataAnnotations;
using NetEscapades.Configuration.Validation;

namespace CoffeeCard.Common.Configuration
{
    public class LoginLimiterSettings : IValidatable
    {
        [Required]
        public bool IsEnabled { get; set; }

        [Required]
        public int MaximumLoginAttemptsWithinTimeOut { get; set; }

        [Required]
        public int TimeOutPeriodInSeconds { get; set; }

        public void Validate()
        {
            Validator.ValidateObject(
                this,
                new ValidationContext(this),
                validateAllProperties: true
            );
        }
    }
}
