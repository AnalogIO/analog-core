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

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgrammeResponse"/> class from a <see cref="Entities.Programme"/> object.
        /// </summary>
        /// <param name="programme">The programme.</param>
        public ProgrammeResponse(Entities.Programme programme)
        {
            Id = programme.Id;
            ShortName = programme.ShortName;
            FullName = programme.FullName;
        }
    }
}