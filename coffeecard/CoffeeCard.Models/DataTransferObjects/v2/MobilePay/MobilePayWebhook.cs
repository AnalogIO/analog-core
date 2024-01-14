using System;

namespace CoffeeCard.Models.DataTransferObjects.v2.MobilePay
{
    /// <summary>
    /// MobilePay webhook invocation request <br />
    /// Code documentation based on <see href="https://mobilepaydev.github.io/MobilePay-Payments-API/docs/webhooks-api">MobilePay Developer: Webhooks</see>
    /// </summary>
    /// <example>
    /// {
    ///   "notificationId": "c85f42aa-0a81-4838-8e87-72236a348d08",
    ///   "eventType": "payment.reserved",
    ///   "eventDate": "2021-10-15T15:30:31Z",
    ///   "data": {
    ///     "id": "ceb351ac-9d20-4300-b5ad-e05851d5a3b7",
    ///     "type": "payment"
    ///   }
    /// }
    /// </example>
    public class MobilePayWebhook
    {
        /// <summary>
        /// Internal MobilePay Id for Webhook invocation
        /// </summary>
        /// <example>c85f42aa-0a81-4838-8e87-72236a348d08</example>
        public string NotificationId { get; set; }

        /// <summary>
        /// Type of event
        /// <list type="table">
        ///     <item>
        ///         <term>payment.reserved</term>
        ///         <description>Published when payment has been approved by MobilePay user and is ready to be captured</description>
        ///     </item>
        ///     <item>
        ///         <term>payment.cancelled_by_user</term>
        ///         <description>Published when payment has been cancelled by user inside MobilePay app</description>
        ///     </item>
        ///     <item>
        ///         <term>payment.expired</term>
        ///         <description>Published when either initiated payment didn't have any user interactions for 5-10 minutes or payment was reserved, but 7 days have passed and the reservation has expired.</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <example>payment.expired</example>
        public string EventType { get; set; }

        /// <summary>
        /// Date time of event dispatch
        /// </summary>
        public DateTime EventDate { get; set; }

        /// <summary>
        /// Event data, e.g. payment id
        /// </summary>
        public EventData Data { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(NotificationId)}: {NotificationId}, {nameof(EventType)}: {EventType}, {nameof(EventDate)}: {EventDate}, {nameof(Data)}: {Data}";
        }
    }

    /// <summary>
    /// Event Data
    /// </summary>
    /// <example>
    /// {
    ///     "id": "ceb351ac-9d20-4300-b5ad-e05851d5a3b7",
    ///     "type": "payment"
    /// }
    /// </example>
    public class EventData
    {
        /// <summary>
        /// MobilePay payment id
        /// </summary>
        /// <example>ceb351ac-9d20-4300-b5ad-e05851d5a3b7</example>
        public string Id { get; set; }

        /// <summary>
        /// Internal MobilePay representation of a type.
        /// </summary>
        /// <example>payment</example>
        public string Type { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Type)}: {Type}";
        }
    }
}
