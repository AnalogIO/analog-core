using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.WebApi.Models.DataTransferObjects.User
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
    ///     "programmeId": 1,
    ///     "level": 1,
    ///     "requiredExp": 12,
    ///     "rankAllTime": 15,
    ///     "rankSemester": 4,
    ///     "rankMonth": 5
    /// }
    /// </example>
    public class UserDto
    {
        /// <summary>
        /// User Id
        /// </summary>
        /// <value>User Id</value>
        /// <example>123</example>
        [Required]
        public int Id { get; set; }
        
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
        public string Email { get; set; }
        
        /// <summary>
        /// Privacy Activated
        /// </summary>
        /// <value>Privacy Activated</value>
        /// <example>true</example>
        [Required]
        public bool PrivacyActivated { get; set; }
        
        /// <summary>
        /// Study Programme Id of user
        /// </summary>
        /// <value>Study Programme Id</value>
        /// <example>1</example>
        [Required]
        public int? ProgrammeId { get; set; }
        
        /// <summary>
        /// User Level
        /// </summary>
        /// <value>User Level</value>
        /// <example>1</example>
        [Required]
        public int Level { get; set; }
        
        /// <summary>
        /// User Experience Level
        /// </summary>
        /// <value>Experience Level</value>
        /// <example>12</example>
        [Required]
        public int RequiredExp { get; set; }
        
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
        public int RankSemester { get; set; }
        
        /// <summary>
        /// User's rank current month
        /// </summary>
        /// <value>Month rank</value>
        /// <example>5</example>
        [Required]
        public int RankMonth { get; set; }
    }
}