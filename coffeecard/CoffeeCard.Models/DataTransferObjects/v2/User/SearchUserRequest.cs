using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.User
{
    /// <summary>
    /// Search a User in the database
    /// </summary>
    /// <example>
    /// {
    ///     "Search": "john.doe"
    /// }
    /// </example>
    public class SearchUserRequest
    {
        /// <summary>
        /// The string that is submitted in a search bar
        /// </summary>
        /// <value> Search of a user  </value>
        /// <example> john.doe@gmail.com </example>
        [Required]
        [MinLength(3)]
        public string Search { get; set; }
    }
}