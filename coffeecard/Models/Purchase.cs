using System;
using System.Collections.Generic;

namespace Coffeecard.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public int Price { get; set; }
        public int NumberOfTickets { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Completed { get; set; }
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
        public virtual User PurchasedBy { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public Purchase() {
            DateCreated = DateTime.UtcNow;
        }
    }
}