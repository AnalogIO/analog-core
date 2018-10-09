using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coffeecard.Models {
    public class User {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public int Experience { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public virtual ICollection<LoginAttempt> LoginAttempts { get; set; }
        public virtual ICollection<Token> Tokens { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
        public bool IsVerified { get; set; }
        public bool PrivacyActivated { get; set; }
        [ForeignKey("Programme_Id")]
        public virtual Programme Programme { get; set; }
        public User() {
            Purchases = new List<Purchase>();
            Tickets = new List<Ticket>();
            Tokens = new List<Token>();
            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;
        }
    }
}