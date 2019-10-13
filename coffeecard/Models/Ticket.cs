using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coffeecard.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUsed { get; set; }
        public int ProductId { get; set; }
        public bool IsUsed { get; set; }
        [ForeignKey("Owner_Id")]
        public virtual User Owner { get; set; }
        [ForeignKey("Purchase_Id")]
        public virtual Purchase Purchase { get; set; }

        public Ticket()
        {
            DateCreated = DateTime.UtcNow;
            DateUsed = null;
        }
    }
}