using System.ComponentModel.DataAnnotations;
using CoffeeCard.Common.Configuration;

namespace CoffeeCard.Common.Models.DataTransferObjects.AppConfig
{
    /// <summary>
    /// App Configuration
    /// </summary>
    /// <example>
    /// {
    ///     "environmentType": "Production",
    ///     "merchantId": "APP1234"
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
        
        /// <summary>
        /// MobilePay MerchantId
        /// </summary>
        /// <value>Merchant Id</value>
        /// <example>APP1234</example>
        [Required]
        public string MerchantId { get; set; }
    }
}