using System;

namespace Coffeecard.Models
{
    public class Token
    {
        public int Id { get; set; }
        public String TokenHash { get; set; }
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