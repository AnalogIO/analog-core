using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeCard.WebApi.Models
{
    public class Token
    {
        public Token(string tokenHash)
        {
            TokenHash = tokenHash;
        }

        public int Id { get; set; }
        public string TokenHash { get; set; }

        [ForeignKey("User_Id")] public virtual User User { get; set; }

        public override bool Equals(object obj)
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