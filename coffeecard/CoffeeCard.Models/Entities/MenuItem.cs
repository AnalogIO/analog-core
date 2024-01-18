using System.Collections.Generic;

namespace CoffeeCard.Models.Entities;
/// <summary>
/// An entry representing an item on the Analog menu
/// e.g. cortado, black coffee, and tea-shirt, etc.
/// </summary>
public class MenuItem
{
    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///  The name of this menuItem
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The products that can consume this menuitem
    /// </summary>
    public ICollection<Product> Products { get; set; }
}