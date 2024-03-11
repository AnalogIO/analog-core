using CoffeeCard.Common.Configuration;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.AppConfig
{
    /// <summary>
    /// App Configuration
    /// </summary>
    /// <example>
    /// {
    ///     "environmentType": "Production"
    /// }
    /// </example>
    public class AppConfigDto
    {
        /// <summary>
        /// Environment type for indicating production or test system
        /// </summary>
        /// <value>Environment Type</value>
        /// <example>Production</example>
        [Required]
        public EnvironmentType EnvironmentType { get; set; }
    }
}