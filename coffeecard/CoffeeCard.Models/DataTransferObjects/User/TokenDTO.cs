namespace CoffeeCard.Models.DataTransferObjects.User
{
    /// <summary>
    /// Login response with Bearer JWT Token
    /// </summary>
    /// <example>
    /// {
    ///     "token": "[no example provided]"
    /// }
    /// </example>
    public class TokenDto
    {
        /// <summary>
        /// Bearer JWT token used for authentication
        /// </summary>
        /// <value>Bearer Token</value>
        /// <example>[no example provided]</example>
        public string? Token { get; set; }
    }
}
