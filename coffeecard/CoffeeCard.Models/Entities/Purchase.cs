using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Models.Entities
{
    [Index(nameof(OrderId), IsUnique = true)]
    public class Purchase
    {
        public Purchase()
        {
            DateCreated = DateTime.UtcNow;
        }

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
        /// <value></value>
        /// <example>2</example>
        
        // FIXME: Foreign reference?
        public int ProductId { get; set; }
        
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
        public DateTime DateCreated { get; set; }
        
        /// <summary>
        /// Purchase completed
        /// </summary>
        /// <value>Purchase Completed</value>
        /// <example>true</example>
        
        // FIXME More detailed state management?
        public bool Completed { get; set; }
        
        /// <summary>
        /// Order Id. Unique identifier used to represent the order at a external provider
        /// </summary>
        /// <value>Order Id</value>
        /// <example>79ef0af3-02dd-4634-83fa-c15ddc05d338</example>
        
        public string OrderId { get; set; }
        
        /// <summary>
        /// Transaction Id from external payment provider
        /// </summary>
        /// <value>Transaction Id</value>
        /// <example>186d2b31-ff25-4414-9fd1-bfe9807fa8b7</example>
        
        // FIXME Uniqueness constraint? Nullable?
        public string TransactionId { get; set; }

        [ForeignKey("PurchasedBy_Id")]
        public virtual User PurchasedBy { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}