using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Models.Entities
{
    [Index(nameof(Email))]
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Password { get; set; } = string.Empty;

        public string Salt { get; set; } = string.Empty;

        public int Experience { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime DateUpdated { get; set; } = DateTime.UtcNow;

        public bool IsVerified { get; set; }

        public bool PrivacyActivated { get; set; }

        [DefaultValue(UserGroup.Customer)]
        public UserGroup UserGroup { get; set; } = UserGroup.Customer;

        public UserState UserState { get; set; }

        [Column(name: "Programme_Id")]
        public int ProgrammeId { get; set; }
        
        public Programme Programme { get; set; }

        public virtual ICollection<LoginAttempt> LoginAttempts { get; set; } = new List<LoginAttempt>();
        
        public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();
        
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        
        public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        
        public virtual ICollection<Statistic> Statistics { get; set; } = new List<Statistic>();

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Email)}: {Email}, {nameof(Name)}: {Name}, {nameof(Password)}: {Password}, {nameof(Salt)}: {Salt}, {nameof(Experience)}: {Experience}, {nameof(DateCreated)}: {DateCreated}, {nameof(DateUpdated)}: {DateUpdated}, {nameof(IsVerified)}: {IsVerified}, {nameof(PrivacyActivated)}: {PrivacyActivated}, {nameof(UserGroup)}: {UserGroup}, {nameof(UserState)}: {UserState}, {nameof(ProgrammeId)}: {ProgrammeId}, {nameof(Programme)}: {Programme}";
        }
    }

    public enum UserState
    {
        Active,
        Deleted,
        PendingActivition
    }
}