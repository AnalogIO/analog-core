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

        public bool IsUsed { get; set; }

        [Column(name: "Owner_Id")]
        public int OwnerId { get; set; }

        public User Owner { get; set; }

        [Column(name: "Purchase_Id")]
        public int PurchaseId { get; set; }

        public Purchase Purchase { get; set; }

        /// <summary>
        /// The id of the menuItem that was consumed
        /// </summary>
        public int? UsedOnMenuItemId { get; set; }
        /// <summary>
        /// The menuItem that was consumed by using the ticket
        /// </summary>
        public MenuItem? UsedOnMenuItem { get; set; }
    }
}