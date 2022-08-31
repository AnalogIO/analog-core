namespace CoffeeCard.Models.DataTransferObjects.v2.Leaderboard
{
    /// <summary>
    /// A user on the leaderboard
    /// </summary>
    /// <example>
    /// {
    ///     "id": "1",
    ///     "name": "John Doe",
    ///     "score": 25
    /// }
    /// </example>
    public class LeaderboardEntry
    {
        /// <summary>
        /// Account Id
        /// </summary>
        /// <value>Account Id</value>
        /// <example>1</example>
        public int Id { get; set; }
        
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