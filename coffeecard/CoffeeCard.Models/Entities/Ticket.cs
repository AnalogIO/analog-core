using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeCard.Models.Entities
{
    public class Ticket
    {
        public int Id { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? DateUsed { get; set; }

        public int ProductId { get; set; }

        /// <summary>
        /// The status of this ticket (e.g. has this ticket been used or refunded?)
        /// </summary>
        public TicketStatus Status { get; set; }

        [Column(name: "Owner_Id")]
        public int OwnerId { get; set; }

        public User Owner { get; set; }

        [Column(name: "Purchase_Id")]
        public int PurchaseId { get; set; }

        public Purchase Purchase { get; set; }

        /// <summary>
        /// The ID of the menu item that this ticket was used on, if any
        /// </summary>
        public int? UsedOnMenuItemId { get; set; }

        /// <summary>
        /// The menu item that this ticket was used on, if any
        /// </summary>
        public MenuItem? UsedOnMenuItem { get; set; }

        /// <summary>
        /// Whether or not this ticket has been used (does not return true for refunded tickets).
        /// </summary>
        /// <remarks>
        /// This is a convenience property that is equivalent to <c>Status == TicketStatus.Used</c>.
        /// </remarks>
        public bool IsUsed => Status == TicketStatus.Used;

        /// <summary>
        /// Whether or not this ticket is unused.
        /// </summary>
        /// <remarks>
        /// This is a convenience property that is equivalent to <c>Status == TicketStatus.Unused</c>.
        /// </remarks>
        public bool IsUnused => Status == TicketStatus.Unused;
    }
}
