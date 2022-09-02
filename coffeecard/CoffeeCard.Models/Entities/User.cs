using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

namespace CoffeeCard.Models.Entities
{
    public class User
    {
        public User()
        {
            Statistics = new List<Statistic>();
            Purchases = new List<Purchase>();
            Tickets = new List<Ticket>();
            Tokens = new List<Token>();
            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;
        }

        [Required] public int Id { get; set; }

        [Required] public string Email { get; set; }

        [Required] public string Name { get; set; }

        [Required] public string Password { get; set; }

        [Required] public string Salt { get; set; }

        public int Experience { get; set; }

        [Required] public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        [Required] public bool IsVerified { get; set; }

        [Required] public bool PrivacyActivated { get; set; }

        [Required]
        [DefaultValue(UserGroup.Customer)]
        public UserGroup UserGroup { get; set; }

        [Required] public UserState UserState { get; set; }

        [ForeignKey("Programme_Id")] public virtual Programme Programme { get; set; }

        public virtual ICollection<LoginAttempt> LoginAttempts { get; set; }
        public virtual ICollection<Token> Tokens { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
        public virtual ICollection<Statistic> Statistics { get; set; }
    }

    public enum UserState
    {
        Active,
        Deleted,
        PendingActivition
    }
}