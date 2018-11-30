using System.ComponentModel.DataAnnotations;

namespace coffeecard.Models.DataTransferObjects.MobilePay
{
    public class InitiatePurchaseDTO
    {
        
        [Required]
        public int ProductId { get; set; }
    }
}