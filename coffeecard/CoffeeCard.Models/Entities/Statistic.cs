using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Models.Entities
{
    /// <summary>
    /// This class will be used to optimize the queries to get statistics.
    /// The preset defines the kind of statistic entity and each preset has a corresponding `SwipeCount`, `SwipeRank` and `ExpiryDate`.
    /// This means that each user should have exactly 3 entries in the database after 1 or more swipes.
    ///
    /// Each preset should be updated accordingly to the current date and the ExpiryDate.
    /// If the ExpiryDate of a entry is exceeded, then the `SwipeCount`, `SwipeRank` and `ExpiryDate` should be reset.
    /// </summary>
    public enum StatisticPreset
    {
        /// <summary>
        /// Statistic preset for a month
        /// </summary>
        Monthly,

        /// <summary>
        /// Statistic preset for a semester
        /// </summary>
        Semester,

        /// <summary>
        /// All Statistics
        /// </summary>
        Total,
    }

    /// <summary>
    /// Statistic represent a user's swipe of tickets. A Statistic entry is valid within a given preset period unless it is the total statistics.
    /// </summary>
    [Index(nameof(Preset), nameof(ExpiryDate))]
    public class Statistic
    {
        /// <summary>
        /// Statistic Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Preset. A uesr is expected to have one entry of each Preset type
        /// </summary>
        public StatisticPreset Preset { get; set; }

        /// <summary>
        /// Number of swipes for preset and within expiry date
        /// </summary>
        public int SwipeCount { get; set; }

        /// <summary>
        /// Last time a user swiped for a given preset
        /// </summary>
        public DateTime LastSwipe { get; set; }

        /// <summary>
        /// Expiry date of statistic entry. When the expiry date has been exceeded, the statistic entry is reest
        /// </summary>
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// User Id which Statistic entry is for
        /// </summary>
        [Column(name: "User_Id")]
        public int UserId { get; set; }

        /// <summary>
        /// Object reference to User
        /// </summary>
        public virtual User User { get; set; }
    }
}
