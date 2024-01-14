namespace CoffeeCard.Models.DataTransferObjects
{
    /// <summary>
    /// Simple response class with a string message
    /// </summary>
    /// <example>
    /// {
    ///     "message": "Successful completion"
    /// }
    /// </example>
    public class MessageResponseDto
    {
        /// <summary>
        /// Message with API response
        /// </summary>
        /// <value>Response</value>
        /// <example>Successful completion</example>
        public string? Message { get; set; }
    }
}
