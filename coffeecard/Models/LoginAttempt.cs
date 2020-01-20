using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeCard.Models
{
    public class LoginAttempt
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        [ForeignKey("User_Id")]
        public virtual User User { get; set; }
        public LoginAttempt()
        {
            Time = DateTime.UtcNow;
        }
    }
}