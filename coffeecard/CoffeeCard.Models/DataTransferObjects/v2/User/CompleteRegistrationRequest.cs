namespace CoffeeCard.Models.DataTransferObjects.v2.User
{
    /// <summary>
    /// Request to complete a registration that was started from a login request.
    /// The email address is read from the single-use registration token and is
    /// deliberately not part of this request.
    /// </summary>
    /// <example>
    /// {
    ///     "token": "[no example provided]",
    ///     "name": "John Doe",
    ///     "programmeId": 1
    /// }
    /// </example>
    public class CompleteRegistrationRequest
    {
        /// <summary>
        /// Token from the registration link sent to the user's email
        /// </summary>
        /// <value>Registration token</value>
        /// <example>[no example provided]</example>
        public required string Token { get; set; }

        /// <summary>
        /// Full name of the user
        /// </summary>
        /// <value>Full Name</value>
        /// <example>John Doe</example>
        public required string Name { get; set; }

        /// <summary>
        /// Programme of the user
        /// </summary>
        /// <value>Programme Id</value>
        /// <example>1</example>
        public required int ProgrammeId { get; set; }
    }
}
