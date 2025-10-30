using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeCard.Models.Entities
{
    /// <summary>
    /// Represents a ticket entity.
    /// </summary>
    public class Ticket
    {
        /// <summary>
        /// Gets or sets the ID of the ticket.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the date the ticket was created.
        /// </summary>
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date the ticket was used.
        /// </summary>
        public DateTime? DateUsed { get; set; }

        /// <summary>
        /// Gets or sets the ID of the product associated with the ticket.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the ticket has been used.
        /// </summary>
        [Obsolete("Use Status instead", true)]
        public bool IsUsed { get; set; }

        /// <summary>
        /// The status of this ticket (e.g. has this ticket been used or refunded?)
        /// </summary>
        public TicketStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the ID of the owner associated with the ticket.
        /// </summary>
        [Column(name: "Owner_Id")]
        public int OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the owner associated with the ticket.
        /// </summary>
        public User Owner { get; set; }

        /// <summary>
        /// Gets or sets the ID of the purchase associated with the ticket.
        /// </summary>
        [Column(name: "Purchase_Id")]
        public int PurchaseId { get; set; }

        /// <summary>
        /// Gets or sets the purchase associated with the ticket.
        /// </summary>
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
        /// Whether or not this ticket has been consumed. Does not return true for refunded tickets.
        /// </summary>
        /// <remarks>
        /// This is a convenience property that is equivalent to <c>Status == TicketStatus.Used</c>.
        /// </remarks>
        public bool IsConsumed => Status == TicketStatus.Used;

        /// <summary>
        /// Whether or not this ticket can be comsumed.
        /// </summary>
        /// <remarks>
        /// This is a convenience property that is equivalent to <c>Status == TicketStatus.Unused</c>.
        /// </remarks>
        public bool IsConsumable => Status == TicketStatus.Unused;
    }
}
