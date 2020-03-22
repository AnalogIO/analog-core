using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.WebApi.Models.PagesModels
{
    public class NewPinCodeModel
    {
        [Required]
        [RegularExpression(@"\d{4}", ErrorMessage = "The pin code must be four digits")]
        [DataType(DataType.Password, ErrorMessage = "The pin code must be four digits")]
        [Display(Name = "new pin code")]
        public string NewPinCode { get; set; }

        [Required]
        [RegularExpression(@"\d{4}", ErrorMessage = "The pin code must be four digits")]
        [DataType(DataType.Password, ErrorMessage = "The pin code must be four digits")]
        [Compare(nameof(NewPinCode), ErrorMessage = "The pin codes must be the same")]
        [Display(Name = "confirmed pin code")]
        public string RepeatedPinCode { get; set; }
    }
}