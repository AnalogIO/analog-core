using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coffeecard.Models
{
    public class Voucher
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUsed { get; set; }
        [ForeignKey("Product_Id")]
        public virtual Product Product { get; set; }
        [ForeignKey("User_Id")]
        public virtual User User { get; set; }

        public Voucher()
        {
            DateCreated = DateTime.UtcNow;
        }
    }
}