using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.PagesModels
{
    /// <summary>
    /// Represents the model for setting a new pin code.
    /// </summary>
    public class NewPinCodeModel
    {
        /// <summary>
        /// Gets or sets the new pin code.
        /// </summary>
        [Required]
        [RegularExpression(@"\d{4}", ErrorMessage = "The pin code must be four digits")]
        [DataType(DataType.Password, ErrorMessage = "The pin code must be four digits")]
        [Display(Name = "new pin code")]
        public string NewPinCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the repeated new pin code, for confirmation.
        /// </summary>
        [Required]
        [RegularExpression(@"\d{4}", ErrorMessage = "The pin code must be four digits")]
        [DataType(DataType.Password, ErrorMessage = "The pin code must be four digits")]
        [Compare(nameof(NewPinCode), ErrorMessage = "The pin codes must be the same")]
        [Display(Name = "confirmed pin code")]
        public string RepeatedPinCode { get; set; } = string.Empty;
    }
}
