using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coffeecard.Models
{
    public class CoffeCard
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int TicketsLeft { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}
