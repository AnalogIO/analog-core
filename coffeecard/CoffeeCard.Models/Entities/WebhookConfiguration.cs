using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.Entities
{
    /// <summary>
    /// MobilePay Webhook configuration
    /// </summary>
    public class WebhookConfiguration
    {
        /// <summary>
        /// Webhook Id at Mobile Pay
        /// </summary>
        [Key]
        [Required]
        public Guid Id { get; set; }
        
        /// <summary>
        /// Url which webhook is registered for
        /// </summary>
        [Required]
        public string Url { get; set; }
        
        /// <summary>
        /// Signature Key
        /// </summary>
        [Required]
        public string SignatureKey { get; set; }

        /// <summary>
        /// Webhook Status
        /// </summary>
        [Required]
        public WebhookStatus Status { get; set; }

        /// <summary>
        /// Date Time Webhook Status was last updated
        /// </summary>
        [Required]
        public DateTime LastUpdated { get; set; }
    }
    
    public enum WebhookStatus
    {
        Active,
        Disabled
    }
}