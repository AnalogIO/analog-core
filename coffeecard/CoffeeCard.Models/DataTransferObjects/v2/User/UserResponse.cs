using System.ComponentModel.DataAnnotations;
using CoffeeCard.Models.DataTransferObjects.v2.Programme;

namespace CoffeeCard.Models.DataTransferObjects.v2.User
{
    /// <summary>
    /// User information
    /// </summary>
    /// <example>
    /// {
    ///     "id": 123,
    ///     "name": "John Doe",
    ///     "email": "john@doe.com",
    ///     "privacyActivated": true,
    ///     "programme": 1,
    ///     "rankAllTime": 15,
    ///     "rankSemester": 4,
    ///     "rankMonth": 5,
    ///     "role": "Barista"
    /// }
    /// </example>
    public class UserResponse
    {
        /// <summary>
        /// User Id
        /// </summary>
        /// <value>User Id</value>
        /// <example>123</example>
        [Required]
        public required int Id { get; set; }

        /// <summary>
        /// Full Name of user
        /// </summary>
        /// <value>Full Name</value>
        /// <example>John Doe</example>
        [Required]
        public required string Name { get; set; }

        /// <summary>
        /// Email of user
        /// </summary>
        /// <value>Email</value>
        /// <example>john@doe.com</example>
        [Required]
        public required string Email { get; set; }

        /// <summary>
        /// Privacy Activated
        /// </summary>
        /// <value>Privacy Activated</value>
        /// <example>true</example>
        [Required]
        public required bool PrivacyActivated { get; set; }

        /// <summary>
        /// User's role
        /// </summary>
        /// <value>Role</value>
        /// <example>Barista</example>
        [Required]
        public required UserRole Role { get; set; } = UserRole.Customer;

        /// <summary>
        /// Study Programme Id of user
        /// </summary>
        /// <value>Study Programme Id</value>
        /// <example>1</example>
        [Required]
        public required ProgrammeResponse Programme { get; set; }

        /// <summary>
        /// User's Rank all time
        /// </summary>
        /// <value>All time rank</value>
        /// <example>15</example>
        [Required]
        public int RankAllTime { get; set; }

        /// <summary>
        /// User's rank current semester
        /// </summary>
        /// <value>Semester rank</value>
        /// <example>4</example>
        [Required]
        public required int RankSemester { get; set; }

        /// <summary>
        /// User's rank current month
        /// </summary>
        /// <value>Month rank</value>
        /// <example>5</example>
        [Required]
        public required int RankMonth { get; set; }
    }
}
