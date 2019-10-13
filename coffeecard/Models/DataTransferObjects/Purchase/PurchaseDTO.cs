using System;

namespace coffeecard.Models.DataTransferObjects.Purchase
{
    public class PurchaseDTO
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
    }
}
