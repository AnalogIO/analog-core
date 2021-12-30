using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Common.Models.DataTransferObjects.User
{
    /// <summary>
    /// Update User information request object
    /// </summary>
    /// <example>
    /// {
    ///     "name": "John Doe",
    ///     "email": "john@doe.com",
    ///     "privacyActivated": true,
    ///     "programmeId": 1,
    ///     "password": "0ffe1abd1a08215353c233d6e009613e95eec4253832a761af28ff37ac5a150c"
    /// }
    /// </example>
    public class UpdateUserDto
    {
        /// <summary>
        /// Full Name of user
        /// </summary>
        /// <value>Full Name</value>
        /// <example>John Doe</example>
        [Required]
        public string Name { get; set; }
        
        /// <summary>
        /// Email of user
        /// </summary>
        /// <value>Email</value>
        /// <example>john@doe.com</example>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        /// <summary>
        /// Privacy Activated
        /// </summary>
        /// <value>Privacy Activated</value>
        /// <example>true</example>
        [Required]
        public bool? PrivacyActivated { get; set; }
        
        /// <summary>
        /// Study Programme Id of user
        /// </summary>
        /// <value>Study Programme Id</value>
        /// <example>1</example>
        [Required]
        public int? ProgrammeId { get; set; }
        
        /// <summary>
        /// Pin Code as first SHA256, then Base64 encoded string
        /// </summary>
        /// <value>Pin code</value>
        /// <example>no example provided</example>
        [Required]
        public string Password { get; set; }
    }
}