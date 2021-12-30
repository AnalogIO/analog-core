using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Common.Models.DataTransferObjects.MobilePay
{
    public class CompletePurchaseDto
    {
        [Required] public string OrderId { get; set; }

        [Required] public string TransactionId { get; set; }
    }
}