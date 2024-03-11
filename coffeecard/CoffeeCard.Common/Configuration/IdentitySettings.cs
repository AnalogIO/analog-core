using NetEscapades.Configuration.Validation;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Common.Configuration
{
    public class IdentitySettings : IValidatable
    {
        [Required]
        public string TokenKey { get; set; }
        [Required]
        public string AdminToken { get; set; }

        [Required]
        public string ApiKey { get; set; }

        public void Validate()
        {
            Validator.ValidateObject(this, new ValidationContext(this), validateAllProperties: true);
        }
    }
}
