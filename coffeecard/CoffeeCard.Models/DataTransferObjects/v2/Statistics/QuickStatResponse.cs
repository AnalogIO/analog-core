using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Statistics
{
    /// <summary>
    /// A small stat card used by the quick statistics endpoint.
    /// </summary>
    /// <example>
    /// {
    ///     "key": "favourite-drink",
    ///     "title": "Your favourite drink",
    ///     "value": "128",
    ///     "supportingText": "Filter Coffee"
    /// }
    /// </example>
    public class QuickStatResponse
    {
        /// <summary>
        /// Stable stat identifier.
        /// </summary>
        [Required]
        public required string Key { get; set; }

        /// <summary>
        /// Card title.
        /// </summary>
        [Required]
        public required string Title { get; set; }

        /// <summary>
        /// Main value shown in the card.
        /// </summary>
        [Required]
        public required int Value { get; set; }

        /// <summary>
        /// Smaller supporting text shown below the main value.
        /// </summary>
        [Required]
        public required string? SupportingText { get; set; }
    }
}
