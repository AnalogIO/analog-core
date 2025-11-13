using CoffeeCard.Models.Entities;

namespace CoffeeCard.Models.DataTransferObjects.v2.User
{
    /// <summary>
    /// Basic User details
    /// </summary>
    public class SimpleUserResponse
    {
        /// <summary>
        /// User Id
        /// </summary>
        /// <example>1</example>
        public required int Id { get; set; }

        /// <summary>
        /// User's Display Name
        /// </summary>
        /// <example>Name</example>
        public required string Name { get; set; }

        /// <summary>
        /// User's Email
        /// </summary>
        /// <example>john@doe.test</example>
        public required string Email { get; set; }

        /// <summary>
        /// User's User group relationship
        /// </summary>
        /// <example>Barista</example>
        public required UserGroup UserGroup { get; set; }

        /// <summary>
        /// User's State
        /// </summary>
        /// <example>Active</example>
        public required UserState State { get; set; }
    }
}
