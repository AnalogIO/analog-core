using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.WebApi.Models.DataTransferObjects.MobilePay
{
    public class InitiatePurchaseDTO
    {
        [Required] public int ProductId { get; set; }
    }
}