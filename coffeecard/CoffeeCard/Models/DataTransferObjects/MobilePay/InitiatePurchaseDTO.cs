using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.MobilePay
{
    public class InitiatePurchaseDTO
    {
        [Required] public int ProductId { get; set; }
    }
}