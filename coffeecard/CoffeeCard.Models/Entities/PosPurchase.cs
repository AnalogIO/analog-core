using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeCard.Models.Entities
{
	public class PosPurhase{
		[Key, ForeignKey(nameof(Purchase))]
		public int PurchaseId { get; set; }
		public virtual Purchase Purchase { get; set; }
		public string BastiaInitials {get; set; }
	}
}