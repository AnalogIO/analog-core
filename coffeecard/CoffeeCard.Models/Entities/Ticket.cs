using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeCard.Models.Entities
{
    public class Ticket
    {
        public Ticket()
        {
            DateCreated = DateTime.UtcNow;
            DateUsed = null;
        }

        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUsed { get; set; }
        public int ProductId { get; set; }
        public bool IsUsed { get; set; }

        [ForeignKey("Owner_Id")] public virtual User Owner { get; set; }

        [ForeignKey("Purchase_Id")] public virtual Purchase Purchase { get; set; }
    }
}