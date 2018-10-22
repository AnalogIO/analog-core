using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeecard.Models.DataTransferObjects.Product
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public int NumberOfTickets { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
