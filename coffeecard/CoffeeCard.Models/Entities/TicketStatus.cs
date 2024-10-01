namespace CoffeeCard.Models.Entities
{
    /// <summary>
    /// Represents the status of a ticket in the system.
    /// </summary>
    public enum TicketStatus
    {
        /// <summary>
        /// Ticket is active and can be used
        /// </summary>
        Unused = 0,

        /// <summary>
        /// Ticket has been used
        /// </summary>
        Used = 1,

        /// <summary>
        /// Ticket has been refunded
        /// </summary>
        Refunded = 2,
    }
}
