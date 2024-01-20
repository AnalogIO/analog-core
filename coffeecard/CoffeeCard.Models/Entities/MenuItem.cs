using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Models.Entities;

/// <summary>
/// An item or good that Analog offers, such as a cappuccino or a t-shirt.
/// </summary>
[Index(nameof(Name), IsUnique = true)]
public class MenuItem
{
    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of this menu item
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The product(s) that are eligible to redeem this menu item
    /// </summary>
    public ICollection<Product> AssociatedProducts { get; set; }
}
