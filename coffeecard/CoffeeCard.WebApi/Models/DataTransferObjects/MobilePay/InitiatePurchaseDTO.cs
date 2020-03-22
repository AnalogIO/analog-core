using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.WebApi.Models.DataTransferObjects.MobilePay
{
    public class InitiatePurchaseDto
    {
        [Required] public int ProductId { get; set; }
    }
}