using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeCard.Models
{
    public class Purchase
    {
        public Purchase()
        {
            DateCreated = DateTime.UtcNow;
        }
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public int Price { get; set; }
        public int NumberOfTickets { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Completed { get; set; }
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
        [ForeignKey("PurchasedBy_Id")]
        public virtual User PurchasedBy { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}