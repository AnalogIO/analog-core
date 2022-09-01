using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeCard.Models.Entities
{

    /**
    * This class will be used to optimize the queries to get statistics.
    * The preset defines the kind of statistic entity and each preset has a corresponding `SwipeCount`, `SwipeRank` and `ExpiryDate`.
    * This means that each user should have exactly 3 entries in the database after 1 or more swipes.
    * Each preset should be updated accordingly to the current date and the ExpiryDate. If the ExpiryDate of a entry is exceeded, then the `SwipeCount`, `SwipeRank` and `ExpiryDate` should be reset.
    **/
    public enum StatisticPreset { Monthly, Semester, Total }

    public class Statistic
    {
        public int Id { get; set; }
        public StatisticPreset Preset { get; set; }
        public int SwipeCount { get; set; }
        public DateTime LastSwipe { get; set; }
        public DateTime ExpiryDate { get; set; }

        [ForeignKey("User_Id")]
        public virtual User User { get; set; }
    }
}