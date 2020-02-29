using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NetEscapades.Configuration.Validation;

namespace CoffeeCard.Configuration
{
    public class EnvironmentSettings : IValidatable
    {
        [Required]
        public EnvironmentType EnvironmentType { get; set; }
        [Required]
        public string MinAppVersion { get; set; }
        [Required]
        public string DeploymentUrl { get; set; }

        public void Validate()
        {
            Validator.ValidateObject(this, new ValidationContext(this), validateAllProperties: true);
        }
    }

    public enum EnvironmentType { Production, Test }
}
