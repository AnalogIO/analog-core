using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Statistics
{
    /// <summary>
    /// The type of quick stat.
    /// </summary>
    public enum QuickStatType
    {
        /// <summary>
        /// The total number of drinks the user has consumed.
        /// </summary>
        [Display(Name = "total-drinks-user")]
        TotalDrinks,

        /// <summary>
        /// The number of drinks consumed by all users today.
        /// </summary>
        [Display(Name = "global-drinks-today")]
        DrinksToday,

        /// <summary>
        /// The user's favourite drink.
        /// </summary>
        [Display(Name = "favourite-drink")]
        FavouriteDrink,

        /// <summary>
        /// The number of drinks the user has consumed this week.
        /// </summary>
        [Display(Name = "drinks-this-week-user")]
        DrinksThisWeek
    }
}