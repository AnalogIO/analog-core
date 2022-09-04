using System;

namespace CoffeeCard.Models.DataTransferObjects.v2.Leaderboard
{
    /// <summary>
    /// A user on the leaderboard
    /// </summary>
    /// <example>
    /// {
    ///     "id": "1",
    ///     "name": "John Doe",
    ///     "rank": 2,
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
        /// Leaderboard rank
        /// </summary>
        /// <value>Rank</value>
        /// <example>2</example>
        public int Rank { get; set; }
        
        /// <summary>
        /// Account score
        /// </summary>
        /// <value>Account score</value>
        /// <example>25</example>
        public int Score { get; set; }

        protected bool Equals(LeaderboardEntry other)
        {
            return Id == other.Id && Name == other.Name && Rank == other.Rank && Score == other.Score;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LeaderboardEntry) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Rank, Score);
        }
    }
}