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
            return $"{nameof(Id)}: {Id}, {nameof(Email)}: {Email}, {nameof(Name)}: {Name}, {nameof(Password)}: {Password}, {nameof(Salt)}: {Salt}, {nameof(Experience)}: {Experience}, {nameof(DateCreated)}: {DateCreated}, {nameof(DateUpdated)}: {DateUpdated}, {nameof(IsVerified)}: {IsVerified}, {nameof(PrivacyActivated)}: {PrivacyActivated}, {nameof(UserGroup)}: {UserGroup}, {nameof(UserState)}: {UserState}, {nameof(ProgrammeId)}: {ProgrammeId}, {nameof(Programme)}: {Programme}";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class with default values.
        /// </summary>
        public User(string email, string name, string password, string salt, Programme programme)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Value cannot be null or empty.", nameof(email));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Value cannot be null or empty.", nameof(password));
            if (string.IsNullOrEmpty(salt))
                throw new ArgumentException("Value cannot be null or empty.", nameof(salt));
            if (programme == null)
                throw new ArgumentNullException(nameof(programme));

            Email = email;
            Name = name;
            Password = password;
            Salt = salt;
            Programme = programme;
            ProgrammeId = programme.Id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class with default values.
        /// </summary>
        public User()
        {
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
        PendingActivition
    }
}
