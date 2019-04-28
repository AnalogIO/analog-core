using System;
using System.Collections.Generic;

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

    class CoffeeCardComparer : IEqualityComparer<CoffeCard>
    {
        // CoffeeCards are equal if their id is equal.
        public bool Equals(CoffeCard x, CoffeCard y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.ProductId == y.ProductId;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(CoffeCard coffeCard)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(coffeCard, null)) return 0;

            //Get hash code for the ProductId field.
            int hashProductCode = coffeCard.ProductId.GetHashCode();

            return hashProductCode;
        }

    }
}
