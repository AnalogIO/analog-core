﻿using System.Collections.Generic;

namespace CoffeeCard.WebApi.Models
{
    public class CoffeeCard
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int TicketsLeft { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }

    internal class CoffeeCardComparer : IEqualityComparer<CoffeeCard>
    {
        // CoffeeCards are equal if their id is equal.
        public bool Equals(CoffeeCard x, CoffeeCard y)
        {
            //Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return x.ProductId == y.ProductId;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(CoffeeCard coffeeCard)
        {
            //Check whether the object is null
            if (ReferenceEquals(coffeeCard, null)) return 0;

            //Get hash code for the ProductId field.
            var hashProductCode = coffeeCard.ProductId.GetHashCode();

            return hashProductCode;
        }
    }
}