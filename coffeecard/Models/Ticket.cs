using System;

namespace Coffeecard.Models {
    public class Ticket {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUsed { get; set; }
        public int ProductId { get; set; }
        public bool IsUsed { get; set; }
        public virtual User Owner { get; set; }
        public virtual Purchase Purchase { get; set; }

        public Ticket() {
            DateCreated = DateTime.UtcNow;
            DateUsed = null;
        }
    }
}