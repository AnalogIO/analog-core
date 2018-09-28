using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coffeecard.Models
{
    public class Voucher
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUsed { get; set; }
        public virtual Product Product { get; set; }
        public virtual User User { get; set; }

        public Voucher()
        {
            DateCreated = DateTime.UtcNow;
        }
    }
}