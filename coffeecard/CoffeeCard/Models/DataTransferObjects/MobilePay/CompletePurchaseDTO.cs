using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.WebApi.Models.DataTransferObjects.MobilePay
{
    public class CompletePurchaseDTO
    {
        [Required] public string OrderId { get; set; }

        [Required] public string TransactionId { get; set; }
    }
}