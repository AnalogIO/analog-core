using System.ComponentModel.DataAnnotations;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Models.DataTransferObjects.v2.User;

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
    public required int AccountId { get; set; }

    /// <summary>
    /// The user group
    /// </summary>
    /// <value> User group </value>
    /// <example>Barista</example>
    [Required]
    public required UserGroup UserGroup { get; set; }
}
