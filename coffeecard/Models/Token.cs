using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeCard.Models
{
    public class Token
    {
        public int Id { get; set; }
        public String TokenHash { get; set; }
        [ForeignKey("User_Id")]
        public virtual User User { get; set; }
        public Token(string tokenHash)
        {
            TokenHash = tokenHash;
        }
        public Token()
        {
            //TokenHash = TokenManager.GenerateToken();
        }
    }
}