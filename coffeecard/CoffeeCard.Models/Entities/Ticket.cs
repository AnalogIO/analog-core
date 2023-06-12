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
        public bool IsUsed { get; set; }

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
        /// Initializes a new instance of the <see cref="Ticket"/> class with the specified associated purchase.
        /// </summary>
        /// <param name="purchase">The purchase associated with the ticket.</param>
        public Ticket(Purchase purchase)
        {
            Purchase = purchase;
            PurchaseId = purchase.Id;
            Owner = purchase.PurchasedBy;
            OwnerId = purchase.PurchasedById;
            ProductId = purchase.ProductId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ticket"/> class.
        /// </summary>
        public Ticket()
        {
        }
    }
}