using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Models.Entities
{
    [Index(nameof(Code), IsUnique = true)]
    public class Voucher
    {
        public Voucher()
        {
            DateCreated = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUsed { get; set; }
        //Description and Requester are nullable for migration purposes
        public string? Description { get; set; }
        public string? Requester {get; set; }
        [ForeignKey("Product_Id")] public virtual Product Product { get; set; }
        [ForeignKey("User_Id")] public virtual User User { get; set; }
    }
}