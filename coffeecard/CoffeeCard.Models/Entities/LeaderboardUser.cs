namespace CoffeeCard.Models.Entities
{
    /// <summary>
    /// A user on the leaderboard
    /// </summary>
    /// <example>
    /// {
    ///     "name": "John Doe",
    ///     "score": 25
    /// }
    /// </example>
    public class LeaderboardUser
    {
        /// <summary>
        /// Account name
        /// </summary>
        /// <value>Account name</value>
        /// <example>John Doe</example>
        public string Name { get; set; }
        
        /// <summary>
        /// Account score
        /// </summary>
        /// <value>Account score</value>
        /// <example>25</example>
        public int Score { get; set; }
    }
}