using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Models.Entities
{
    /// <summary>
    /// User entity with its related properties.
    /// </summary>
    [Index(nameof(Email))]
    [Index(nameof(Name))]
    [Index(nameof(UserGroup))]
    public class User
    {
        /// <summary>
        /// Gets or sets the user's Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user's email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's salt.
        /// </summary>
        public string Salt { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's experience.
        /// </summary>
        public int Experience { get; set; }

        /// <summary>
        /// Gets or sets the date the user was created.
        /// </summary>
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date the user was last updated.
        /// </summary>
        public DateTime DateUpdated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets a value indicating whether the user is verified.
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user's privacy is activated.
        /// </summary>
        public bool PrivacyActivated { get; set; }

        /// <summary>
        /// Gets or sets the user's group.
        /// </summary>
        [DefaultValue(UserGroup.Customer)]
        public UserGroup UserGroup { get; set; } = UserGroup.Customer;

        /// <summary>
        /// Gets or sets the user's state.
        /// </summary>
        public UserState UserState { get; set; }

        /// <summary>
        /// Gets or sets the Programme Id.
        /// </summary>
        [Column(name: "Programme_Id")]
        public int ProgrammeId { get; set; }

        /// <summary>
        /// Gets or sets the user's programme.
        /// </summary>
        public Programme Programme { get; set; }

        /// <summary>
        /// Gets or sets the user's tokens.
        /// </summary>
        public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();

        /// <summary>
        /// Gets or sets the user's tickets.
        /// </summary>
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        /// <summary>
        /// Gets or sets the user's purchases.
        /// </summary>
        public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

        /// <summary>
        /// Gets or sets the user's statistics.
        /// </summary>
        public virtual ICollection<Statistic> Statistics { get; set; } = new List<Statistic>();

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Email)}: {Email}, {nameof(Name)}: {Name}, {nameof(Password)}: [Redacted], {nameof(Salt)}: [Redacted], {nameof(Experience)}: {Experience}, {nameof(DateCreated)}: {DateCreated}, {nameof(DateUpdated)}: {DateUpdated}, {nameof(IsVerified)}: {IsVerified}, {nameof(PrivacyActivated)}: {PrivacyActivated}, {nameof(UserGroup)}: {UserGroup}, {nameof(UserState)}: {UserState}, {nameof(ProgrammeId)}: {ProgrammeId}, {nameof(Programme)}: {Programme}";
        }
    }

    /// <summary>
    /// Represents the state of a User.
    /// </summary>
    public enum UserState
    {
        /// <summary>
        /// Represents a user who is active.
        /// </summary>
        Active,

        /// <summary>
        /// Represents a user who has been deleted.
        /// </summary>
        Deleted,

        /// <summary>
        /// Represents a user who is pending activation.
        /// </summary>
        PendingActivition,
    }
}
