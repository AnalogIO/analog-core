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
    public sealed class LeaderboardEntry : IEquatable<LeaderboardEntry>
    {
        /// <summary>
        /// Account Id
        /// </summary>
        /// <value>Account Id</value>
        /// <example>1</example>
        public required int Id { get; set; }

        /// <summary>
        /// Account name
        /// </summary>
        /// <value>Account name</value>
        /// <example>John Doe</example>
        public required string Name { get; set; }

        /// <summary>
        /// Leaderboard rank
        /// </summary>
        /// <value>Rank</value>
        /// <example>2</example>
        public required int Rank { get; set; }

        /// <summary>
        /// Account score
        /// </summary>
        /// <value>Account score</value>
        /// <example>25</example>
        public required int Score { get; set; }

        /// <inheritdoc/>
        public bool Equals(LeaderboardEntry? other)
        {
            return other != null
                && Id == other.Id
                && Name == other.Name
                && Rank == other.Rank
                && Score == other.Score;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((LeaderboardEntry)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Rank, Score);
        }
    }
}
