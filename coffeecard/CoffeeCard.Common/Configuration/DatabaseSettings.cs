using System.ComponentModel.DataAnnotations;
using NetEscapades.Configuration.Validation;

namespace CoffeeCard.Common.Configuration
{
    public class DatabaseSettings : IValidatable
    {
        [Required]
        public string ConnectionString { get; set; }

        [Required]
        public string SchemaName { get; set; }

        public void Validate()
        {
            Validator.ValidateObject(this, new ValidationContext(this), true);
        }
    }
}
