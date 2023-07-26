using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeCard.Models.Entities
{
    /// Represents a purchase issued by a barista at the point of sale in the cafe
	public class PosPurhase{
		/// The foreign key linking to a purchase
		[Key, ForeignKey(nameof(Purchase))]
		public int PurchaseId { get; set; }
        /// The purchase that was issued by the barista
		public virtual Purchase Purchase { get; set; }
        /// The ITU intials of the issueing barista
		public string BastiaInitials {get; set; }
	}
}