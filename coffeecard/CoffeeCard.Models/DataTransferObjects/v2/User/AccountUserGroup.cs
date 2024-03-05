using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.Entities;

/// <summary>
/// Represents an account user group update
/// </summary>
public class AccountUserGroup
{
    /// <summary>
    /// The account id
    /// </summary>
    /// <value> Account id </value>
    /// <example>1</example>
    [Required]
    public int AccountId { get; set; }

    /// <summary>
    /// The user group
    /// </summary>
    /// <value> User group </value>
    /// <example>Barista</example>
    [Required]
    public UserGroup UserGroup { get; set; }
}
