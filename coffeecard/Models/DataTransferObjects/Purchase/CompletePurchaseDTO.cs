using System.ComponentModel.DataAnnotations;
namespace coffeecard.Models.DataTransferObjects.Purchase
{
    public class CompletePurchaseDTO {
        [Required]
        public string OrderId { get; set; }
        [Required]
        public string TransactionId { get; set; }
    }
}