using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeCard.Models.Entities
{
    /// Represents a purchase issued by a barista with the point of sale system in the cafe
    public class PosPurhase
    {
        /// The foreign key linking to a purchase
        [Key, ForeignKey(nameof(Purchase))]
        public int PurchaseId { get; set; }

        /// The purchase that was issued by the barista
        public virtual required Purchase Purchase { get; set; }

        /// The ITU intials of the barista issueing the product
        public required string BaristaInitials { get; set; }
    }
}
