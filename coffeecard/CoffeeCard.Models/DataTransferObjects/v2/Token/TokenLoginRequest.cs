using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Token
{
    /// <summary>
    /// Magic link request object
    /// </summary>
    /// <example>
    /// {
    ///     "token": "[no example provided]",
    /// }
    /// </example>
    public class TokenLoginRequest
    {
        /// <summary>
        /// Magic link token
        /// </summary>
        /// <value>Token</value>
        /// <example>[no example provided]</example>
        [Required]
        public string Token { get; set; } = null!;
    }
}
