using System;

namespace Coffeecard.Models
{
    public class LoginAttempt
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public virtual User User { get; set; }
        public LoginAttempt()
        {
            Time = DateTime.UtcNow;
        }
    }
}