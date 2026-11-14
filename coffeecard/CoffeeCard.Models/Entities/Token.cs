using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Models.Entities
{
    /// <summary>
    /// Shared Token class for different token types
    /// </summary>
    [Index(nameof(TokenHash))]
    public class Token(string tokenHash, TokenType type)
    {
        /// <summary>
        /// The ID of the token
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The randomly generated hash used to find the token
        /// </summary>
        [MaxLength(1000)]
        public string TokenHash { get; set; } = tokenHash;

        /// <summary>
        /// The ID of the user that the token is associated with
        /// </summary>
        [Column(name: "User_Id")]
        public int? UserId { get; set; }

        /// <summary>
        /// The user that the token is associated with
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// The type of token
        /// </summary>
        /// <example>
        /// RefreshToken
        /// </example>
        public TokenType Type { get; set; } = type;

        /// <summary>
        /// The date and time when the Token is no longer valid
        /// </summary>
        public DateTime Expires { get; set; } = type.getExpiresAt();

        /// <summary>
        /// Whether or not the token has been revoked
        /// </summary>
        public bool Revoked { get; set; } = false;

        /// <summary>
        /// Determines if two tokens are equal
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is Token newToken)
                return TokenHash.Equals(newToken.TokenHash);
            return false;
        }

        /// <summary>
        /// Gets the hash code of the token
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, TokenHash, User);
        }

        /// <summary>
        /// Determines if the token has expired
        /// </summary>
        /// <returns>bool</returns>
        [DbFunction("Expired", "dbo")]
        public static bool Expired(DateTime expires)
        {
            return DateTime.UtcNow > expires;
        }
    }
}
