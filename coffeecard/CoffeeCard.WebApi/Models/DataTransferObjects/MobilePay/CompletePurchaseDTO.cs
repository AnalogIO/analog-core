using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.WebApi.Models.DataTransferObjects.MobilePay
{
    public class CompletePurchaseDto
    {
        [Required] public string OrderId { get; set; }

        [Required] public string TransactionId { get; set; }
    }
}