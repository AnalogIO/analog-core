using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Models.Entities
{
    // FIXME Should me marked as unique
    [Index(nameof(OrderId))]
    [Index(nameof(ExternalTransactionId))]
    public class Purchase
    {
        /// <summary>
        /// Purchase Id
        /// </summary>
        /// <value>Purchase Id</value>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Name of product purchased
        /// </summary>
        /// <value>Product Name</value>
        /// <example>Coffee clip card</example>
        public string ProductName { get; set; }

        /// <summary>
        /// Id of Product purchased
        /// </summary>
        /// <value>Product Id</value>
        /// <example>2</example>
        public int ProductId { get; set; }

        /// <summary>
        /// Product purchased
        /// </summary>
        /// <value>Product</value>
        public Product Product { get; set; }

        /// <summary>
        /// Price for purchase in Danish kroner (kr)
        /// </summary>
        /// <value>Price in Danish Kroner</value>
        /// <example>100</example>
        public int Price { get; set; }

        /// <summary>
        /// Number of tickets issued in purchase
        /// </summary>
        /// <value>Tickets issued</value>
        /// <example>10</example>
        public int NumberOfTickets { get; set; }

        /// <summary>
        /// Date purchase was created
        /// </summary>
        /// <value>Purchase DateCreated</value>
        /// <example>???</example>
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Purchase completed
        /// </summary>
        /// <value>Purchase Completed</value>
        /// <example>true</example>
        public bool Completed { get; set; }

        /// <summary>
        /// Order Id. Unique identifier used to represent the order at a external provider
        /// </summary>
        /// <value>Order Id</value>
        /// <example>79ef0af3-02dd-4634-83fa-c15ddc05d338</example>
        // TODO Change type to GUID, so it maps to uniqueuIdentifier in ef
        public string OrderId { get; set; }

        /// <summary>
        /// Transaction Id from external payment provider
        /// </summary>
        /// <value>Transaction Id</value>
        /// <example>186d2b31-ff25-4414-9fd1-bfe9807fa8b7</example>
        public string? ExternalTransactionId { get; set; }

        /// <summary>
        /// Purchase Status. The status should also reflect the status at the external payment provider
        /// </summary>
        /// <value>Status</value>
        /// <example>Completed</example>
        public PurchaseStatus Status { get; set; }

        // PaymentType is nullable for migration purposes
        /// <summary>
        /// Payment Type
        /// </summary>
        /// <value>Payment Type</value>
        /// <example>MobilePay</example>
        public PaymentType? PaymentType { get; set; }

        /// The type of purchase e.g. MobilePayV1, Free
        public PurchaseType Type { get; set; }

        [Column(name: "PurchasedBy_Id")]
        public int PurchasedById { get; set; }

        public User PurchasedBy { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}