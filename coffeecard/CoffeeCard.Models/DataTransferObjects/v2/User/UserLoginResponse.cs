namespace CoffeeCard.Models.DataTransferObjects.v2.User
{
    /// <summary>
    /// User login response object
    /// </summary>
    /// <example>
    public class UserLoginResponse
    {
        /// <summary>
        /// JSON Web Token with claims for the user logging in
        /// </summary>
        public required string Jwt { get; set; }

        /// <summary>
        /// User's Display Name
        /// </summary>
        /// <example>Name</example>
        public required string RefreshToken { get; set; }
    }
}
