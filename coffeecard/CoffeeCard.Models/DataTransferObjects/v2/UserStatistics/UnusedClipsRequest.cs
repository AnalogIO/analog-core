using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.UserStatistics
{
    /// <summary>
    /// Initialize an request for data with unused clips 
    /// </summary>
    public class UnusedClipsRequest
    {
        /// <summary>
        /// The start date of unused tickets query
        /// </summary>
        /// <value> Start Date </value>
        /// <example> 2021-02-08 </example>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end date of unused tickets query
        /// </summary>
        /// <value> End Date </value>
        /// <example> 2024-02-08 </example>
        [Required]
        public DateTime EndDate { get; set; }
    }
}

