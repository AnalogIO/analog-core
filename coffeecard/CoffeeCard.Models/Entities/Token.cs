using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeCard.Models.Entities
{
    public class Token(string tokenHash, TokenType type, DateTime expiresAt)
    {
        public int Id { get; set; }

        public string TokenHash { get; set; } = tokenHash;

        [Column(name: "User_Id")]
        public int? UserId { get; set; }

        public User? User { get; set; }

        public TokenType Type { get; set; } = type;

        public DateTime Expires { get; set; } = expiresAt;
        
        
        public override bool Equals(object? obj)
        {
            if (obj is Token newToken) return TokenHash.Equals(newToken.TokenHash);
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, TokenHash, User);
        }
    }
}