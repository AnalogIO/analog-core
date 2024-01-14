using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Models.Entities
{
    [Index(nameof(Code), IsUnique = true)]
    public class Voucher
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? DateUsed { get; set; }

        //Description and Requester are nullable for migration purposes
        public string? Description { get; set; }

        public string? Requester { get; set; }

        [Column(name: "Product_Id")]
        public int ProductId { get; set; }

        public Product Product { get; set; }

        [Column(name: "User_Id")]
        public int? UserId { get; set; }

        public User? User { get; set; }

        public int? PurchaseId { get; set; }

        public virtual Purchase? Purchase { get; set; }
    }
}
