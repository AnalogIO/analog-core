using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Programme
{
    /// <summary>
    /// Represents a study programme
    /// </summary>
    /// <example>
    /// {
    ///     "id": 1,
    ///     "shortName": "SWU",
    ///     "fullName": "Software Development"
    /// }
    /// </example>
    public class ProgrammeResponse
    {
        /// <summary>
        /// Id of study programme
        /// </summary>
        /// <value>Study Programme Id</value>
        /// <example>1</example>
        [Required]
        public required int Id { get; set; }

        /// <summary>
        /// Short name of study programme
        /// </summary>
        /// <example>SWU</example>
        [Required]
        public required string ShortName { get; set; }

        /// <summary>
        /// Full name of study programme
        /// </summary>
        /// <value>Full name</value>
        /// <example>Software development</example>
        [Required]
        public required string FullName { get; set; }
    }
}
