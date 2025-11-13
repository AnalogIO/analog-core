using System.ComponentModel.DataAnnotations;
using NetEscapades.Configuration.Validation;

namespace CoffeeCard.Common.Configuration
{
    public class EnvironmentSettings : IValidatable
    {
        [Required]
        public EnvironmentType EnvironmentType { get; set; }

        [Required]
        public string MinAppVersion { get; set; }

        [Required]
        public string DeploymentUrl { get; set; }

        [Required]
        public string ShiftyUrl { get; set; }

        public void Validate()
        {
            Validator.ValidateObject(this, new ValidationContext(this), true);
        }
    }

    public enum EnvironmentType
    {
        Production,
        Test,
        LocalDevelopment,
    }
}
