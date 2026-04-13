using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CoffeeCard.Models.DataTransferObjects.v2.User
{
    /// <summary>
    /// Update User information request object. All properties are optional as the server only updates the values of the properties which are present
    /// </summary>
    /// <example>
    /// {
    ///     "name": "John Doe",
    ///     "email": "john@doe.com",
    ///     "privacyActivated": true,
    ///     "programmeId": 1,
    ///     "password": "[no example provided]"
    /// }
    /// </example>
    public class UpdateUserRequest
    {
        /// <summary>
        /// Full Name of user
        /// </summary>
        /// <value>Full Name</value>
        /// <example>John Doe</example>
        public string? Name { get; set; }

        /// <summary>
        /// Email of user
        /// </summary>
        /// <value>Email</value>
        /// <example>john@doe.com</example>
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// Privacy Activated
        /// </summary>
        /// <value>Privacy Activated</value>
        /// <example>true</example>
        public bool? PrivacyActivated { get; set; }

        /// <summary>
        /// Study Programme Id of user
        /// </summary>
        /// <value>Study Programme Id</value>
        /// <example>1</example>
        public int? ProgrammeId { get; set; }

        /// <summary>
        /// Pin Code as first UTF8 encoded, then SHA256 hashed, and then Base64 encoded string
        /// </summary>
        /// <value>Pin code</value>
        /// <example>[no example provided]</example>
        public string? Password { get; set; }

        /// <summary>
        /// Profile Icon for the user
        /// </summary>
        /// <value>Profile Icon</value>
        /// <example>4</example>
        public ProfileIcon? ProfileIcon { get; set; }

        /// <summary>
        /// Which background color is used for the profile picture
        /// </summary>
        /// <value>Background Color</value>
        /// <example>Moss Green</example>
        public PictureBackgroundColor? BackgroundColor { get; set; }
    }
}
