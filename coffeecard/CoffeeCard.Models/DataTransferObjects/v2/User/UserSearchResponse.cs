using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.User;

/// <summary>
/// Represents a search result
/// </summary>
public class UserSearchResponse
{
    /// <summary>
    /// The number of users that match the query
    /// </summary>
    /// <value> Users number </value>
    /// <example>1</example>
    [Required]
    public required int TotalUsers { get; set; }

    /// <summary>
    /// The users that match the query
    /// </summary>
    /// <value> Users List </value>
    /// <example>
    /// [
    ///    {
    ///      "id": 12232,
    ///      "name": "John Doe",
    ///      "email": "johndoe@itu.dk",
    ///      "userGroup": "Barista",
    ///      "state": "Active"
    ///    }
    ///  ]
    /// </example>
    [Required]
    public required IEnumerable<SimpleUserResponse> Users { get; set; }
}
