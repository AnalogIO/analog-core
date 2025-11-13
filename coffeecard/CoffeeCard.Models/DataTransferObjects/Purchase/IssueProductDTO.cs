using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.Purchase
{
    /// <summary>
    /// Issue product request
    /// </summary>
    /// <example>
    /// {
    ///     "issuedBy": "dann",
    ///     "userId": "122",
    ///     "productId": 1
    /// }
    /// </example>
    public class IssueProductDto
    {
        /// <summary>
        /// Name/Initials of person who issued purchase
        /// </summary>
        /// <value>Name of issuer</value>
        /// <value>dann</value>
        [Required]
        public required string IssuedBy { get; set; } = string.Empty;

        /// <summary>
        /// User id who should receive the product
        /// </summary>
        /// <value>User id</value>
        /// <example>122</example>
        [Required]
        public required int UserId { get; set; }

        /// <summary>
        /// Id of Product to issue
        /// </summary>
        /// <value>Product Id</value>
        /// <example>1</example>
        [Required]
        public required int ProductId { get; set; }
    }
}
