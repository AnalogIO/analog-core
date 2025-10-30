using System.Collections.Generic;

namespace CoffeeCard.Models.Entities
{
    /// <summary>
    /// Represents an occupation or a programme at ITU.
    /// </summary>
    public class Programme
    {
        /// <summary>
        /// Gets or sets the ID of the programme.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the short name (abbreviation) of the programme.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Gets or sets the full name of the programme.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the sort priority of the programme.
        /// </summary>
        public int SortPriority { get; set; }

        /// <summary>
        /// Gets or sets the collection of users associated with the programme.
        /// </summary>
        public virtual ICollection<User> Users { get; set; } = new List<User>();

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(ShortName)}: {ShortName}, {nameof(FullName)}: {FullName}, {nameof(SortPriority)}: {SortPriority}";
        }
    }
}
