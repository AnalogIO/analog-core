using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
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
    public required string Name { get; set; }

    /// <summary>
    /// The product(s) that are eligible to redeem this menu item
    /// </summary>
    public ICollection<Product> AssociatedProducts { get; set; } = [];

    /// <summary>
    /// Navigational property for the join table between Menu Items and Products
    /// </summary>
    public ICollection<MenuItemProduct> MenuItemProducts { get; set; } = [];

    /// <summary>
    /// Whether or not this menu item is active
    /// </summary>
    public bool Active { get; set; } = true;
}
