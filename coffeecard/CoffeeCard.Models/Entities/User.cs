using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeCard.Models.Entities
{
    [Index(nameof(Email))]
    [Index(nameof(Name))]
    [Index(nameof(UserGroup))]
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

        public virtual ICollection<LoginAttempt> LoginAttempts { get; set; } = [];

        public virtual ICollection<Token> Tokens { get; set; } = [];

        public virtual ICollection<Ticket> Tickets { get; set; } = [];

        public virtual ICollection<Purchase> Purchases { get; set; } = [];

        public virtual ICollection<Statistic> Statistics { get; set; } = [];

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Email)}: {Email}, {nameof(Name)}: {Name}, {nameof(Password)}: [Redacted], {nameof(Salt)}: [Redacted], {nameof(Experience)}: {Experience}, {nameof(DateCreated)}: {DateCreated}, {nameof(DateUpdated)}: {DateUpdated}, {nameof(IsVerified)}: {IsVerified}, {nameof(PrivacyActivated)}: {PrivacyActivated}, {nameof(UserGroup)}: {UserGroup}, {nameof(UserState)}: {UserState}, {nameof(ProgrammeId)}: {ProgrammeId}, {nameof(Programme)}: {Programme}";
        }
    }

    public enum UserState
    {
        Active,
        Deleted,
        PendingActivition
    }
}