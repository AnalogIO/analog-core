using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeCard.Models.Entities
{
    /// <summary>
    /// Represents a token used for authentication and authorization.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class with the specified token hash.
        /// </summary>
        /// <param name="tokenHash">The token hash.</param>
        public Token(string tokenHash)
        {
            TokenHash = tokenHash;
        }

        /// <summary>
        /// Gets or sets the ID of the token.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the token hash.
        /// </summary>
        public string TokenHash { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user associated with the token.
        /// </summary>
        [Column(name: "User_Id")]
        public int? UserId { get; set; }

        /// <summary>
        /// Gets or sets the user associated with the token.
        /// </summary>
        public User? User { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is Token newToken) return TokenHash.Equals(newToken.TokenHash);
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, TokenHash, User);
        }
    }
}