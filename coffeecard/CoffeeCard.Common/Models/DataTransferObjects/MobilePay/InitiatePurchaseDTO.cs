using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Common.Models.DataTransferObjects.MobilePay
{
    public class InitiatePurchaseDto
    {
        [Required] public int ProductId { get; set; }
    }
}