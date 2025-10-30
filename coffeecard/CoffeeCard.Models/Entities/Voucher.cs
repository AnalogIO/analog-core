using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Models.Entities
{
    /// <summary>
    /// Represents a voucher entity.
    /// </summary>
    [Index(nameof(Code), IsUnique = true)]
    public class Voucher
    {
        /// <summary>
        /// Gets or sets the ID of the voucher.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the code of the voucher.
        /// </summary>
        /// <remarks>
        /// This property is required and must be unique.
        /// </remarks>
        [Required]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the date the voucher was created.
        /// </summary>
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date the voucher was used.
        /// </summary>
        public DateTime? DateUsed { get; set; }

        /// <summary>
        /// Gets or sets the description of the voucher.
        /// </summary>
        /// <remarks>
        /// This property is optional.
        /// </remarks>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the requester of the voucher.
        /// </summary>
        /// <remarks>
        /// This property is optional.
        /// </remarks>
        public string? Requester { get; set; }

        /// <summary>
        /// Gets or sets the ID of the product associated with the voucher.
        /// </summary>
        [Column(name: "Product_Id")]
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product associated with the voucher.
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user associated with the voucher.
        /// </summary>
        [Column(name: "User_Id")]
        public int? UserId { get; set; }

        /// <summary>
        /// Gets or sets the user associated with the voucher.
        /// </summary>
        public User? User { get; set; }

        public int? PurchaseId { get; set; }

        public virtual Purchase? Purchase { get; set; }
    }
}
