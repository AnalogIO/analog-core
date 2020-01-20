using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeCard.Models
{
    public class Token
    {
        public Token(string tokenHash)
        {
            TokenHash = tokenHash;
        }

        public Token()
        {
            //TokenHash = TokenManager.GenerateToken();
        }

        public int Id { get; set; }
        public string TokenHash { get; set; }

        [ForeignKey("User_Id")] public virtual User User { get; set; }
    }
}