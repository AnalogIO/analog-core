using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.User
{
    /// <summary>
    /// User login response object
    /// </summary>
    /// <example>
    /// {
    ///     "jwt": "[no example provided]",
    ///     "refreshToken": "[no example provided]"
    /// }
    /// </example>
    public class UserLoginResponse
    {
        /// <summary>
        /// JSON Web Token with claims for the user logging in
        /// </summary>
        [Required]
        public required string Jwt { get; set; }

        /// <summary>
        /// Token used to obtain a new JWT token on expiration
        /// </summary>
        [Required]
        public required string RefreshToken { get; set; }
    }
}
