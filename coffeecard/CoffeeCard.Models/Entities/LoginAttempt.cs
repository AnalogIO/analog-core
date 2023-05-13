using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeCard.Models.Entities
{
    // TODO: Remove this entity since it looks like it's not actively used
    
    public class LoginAttempt
    {
        public LoginAttempt()
        {
            Time = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public DateTime Time { get; set; }

        [ForeignKey("User_Id")] public virtual User User { get; set; }
    }
}