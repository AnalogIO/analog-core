using System.ComponentModel.DataAnnotations;
using NetEscapades.Configuration.Validation;

namespace CoffeeCard.Common.Configuration
{
    public class IdentitySettings : IValidatable
    {
        [Required]
        public string TokenKey { get; set; }
        [Required]
        public string AdminToken { get; set; }
        [Required]
        public int MaximumLoginAttemptsWithinTimeOut { get; set; }
        [Required]
        public int TimeOutPeriodInMinutes { get; set; }

        public void Validate()
        {
            Validator.ValidateObject(this, new ValidationContext(this), validateAllProperties: true);
        }
    }
}
