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
        public Guid Id { get; set; }

        /// <summary>
        /// Url which webhook is registered for
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Signature Key
        /// </summary>
        public string SignatureKey { get; set; } = string.Empty;

        /// <summary>
        /// Webhook Status
        /// </summary>
        public WebhookStatus Status { get; set; }

        /// <summary>
        /// Date Time Webhook Status was last updated
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// Enum representing the status of a MobilePay webhook
    /// </summary>
    public enum WebhookStatus
    {
        /// Webhook is active
        Active,

        /// Webhook is disabled
        Disabled,
    }
}
