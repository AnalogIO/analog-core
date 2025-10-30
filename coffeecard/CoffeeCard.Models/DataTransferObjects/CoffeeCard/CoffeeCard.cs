using System.Collections.Generic;

namespace CoffeeCard.Models.DataTransferObjects.CoffeeCard
{
    /// <summary>
    /// A CoffeeCard is a union datatype of a product and unused tickets associated with the product.
    /// </summary>
    /// <example>
    /// {
    ///     "productId": 1,
    ///     "name": "Filter Coffee",
    ///     "ticketsLeft": 5,
    ///     "price": 50,
    ///     "quantity": 10
    /// }
    /// </example>
    public class CoffeeCard
    {
        /// <summary>
        /// Id of product
        /// </summary>
        /// <value>Product Id</value>
        /// <example>1</example>
        public required int ProductId { get; set; }

        /// <summary>
        /// Name of product
        /// </summary>
        /// <value>Product Name</value>
        /// <example>Filter Coffee</example>
        public required string Name { get; set; }

        /// <summary>
        /// Remaining (if any) unused tickets left for product
        /// </summary>
        /// <value>Remaining Tickets</value>
        /// <example>5</example>
        public required int TicketsLeft { get; set; }

        /// <summary>
        /// Price of product
        /// </summary>
        /// <value>Product Price</value>
        /// <example>50</example>
        public required int Price { get; set; }

        /// <summary>
        /// Quantity of tickets in product
        /// </summary>
        /// <value>Quantity</value>
        /// <example>10</example>
        public required int Quantity { get; set; }
    }

    /// <summary>
    /// Coffee Card comparison class implementing <see cref="IEqualityComparer{T}"/>
    /// </summary>
    public class CoffeeCardComparer : IEqualityComparer<CoffeeCard>
    {
        /// <summary>
        /// CoffeeCards are equal if their product id is equal
        /// </summary>
        public bool Equals(CoffeeCard? x, CoffeeCard? y)
        {
            //Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y))
                return true;

            //Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return x.ProductId == y.ProductId;
        }

        /// <summary>
        /// If Equals() returns true for a pair of objects
        /// then GetHashCode() must return the same value for these objects.
        /// </summary>
        public int GetHashCode(CoffeeCard coffeeCard)
        {
            //Check whether the object is null
            if (ReferenceEquals(coffeeCard, null))
                return 0;

            //Get hash code for the ProductId field.
            var hashProductCode = coffeeCard.ProductId.GetHashCode();

            return hashProductCode;
        }
    }
}
