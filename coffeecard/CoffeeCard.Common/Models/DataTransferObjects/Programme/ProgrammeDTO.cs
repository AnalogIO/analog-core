using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Common.Models.DataTransferObjects.Programme
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
    public class ProgrammeDto
    {
        /// <summary>
        /// Id of study programme
        /// </summary>
        /// <value>Study Programme Id</value>
        /// <example>1</example>
        [Required]
        public int Id { get; set; }
        
        /// <summary>
        /// Short name of study programme
        /// </summary>
        /// <example>SWU</example>
        [Required]
        public string ShortName { get; set; }
        
        /// <summary>
        /// Full name of study programme
        /// </summary>
        /// <value>Full name</value>
        /// <example>Software development</example>
        [Required]
        public string FullName { get; set; }
    }
}