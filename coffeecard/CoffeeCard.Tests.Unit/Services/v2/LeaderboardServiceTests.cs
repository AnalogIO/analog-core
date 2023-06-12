using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.v2.Leaderboard;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services.v2
{
    public class LeaderboardServiceTests
    {
        private static User testuser1 => new User(
            email: "email@email.test",
            name: "User1",
            password: "password",
            salt: "salt",
            programme: new Programme(fullName: "fullName", shortName: "shortName")
        )
        {
            Id = 1,
            DateCreated = new DateTime(year: 2020, month: 11, day: 11),
            IsVerified = true,
            PrivacyActivated = false,
            UserGroup = UserGroup.Customer,
            UserState = UserState.Active
        };

        private static User testuser2 => new User(
            email: "email2@email.test",
            name: "User2",
            password: testuser1.Password,
            salt: testuser1.Salt,
            programme: testuser1.Programme
        )
        {
            Id = 2,
            DateCreated = testuser1.DateCreated,
            IsVerified = testuser1.IsVerified,
            PrivacyActivated = testuser1.PrivacyActivated,
            UserGroup = testuser1.UserGroup,
            UserState = testuser1.UserState
        };

        private static Statistic getStatistic(int id, User user, StatisticPreset preset) => new Statistic(user)
        {
            Id = id,
            Preset = preset,
            SwipeCount = 10,
            LastSwipe = new DateTime(year: 2020, month: 11, day: 11),
            ExpiryDate = new DateTime(year: 2020, month: 11, day: 11),
        };

        [Theory(DisplayName = "GetLeaderboardEntry given user and preset returns Leaderboard Entry")]
        [InlineData(LeaderboardPreset.Month)]
        [InlineData(LeaderboardPreset.Semester)]
        [InlineData(LeaderboardPreset.Total)]
        public async Task GetLeaderboardEntryGivenUserAndPresetReturnsLeaderboardEntry(
            LeaderboardPreset inputPreset)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetLeaderboardEntryGivenUserAndPresetReturnsLeaderboardEntry) +
                                     inputPreset);

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            var user1 = testuser1;
            context.Add(user1);

            var user2 = testuser2;
            context.Add(user2);

            var statPreset = inputPreset.ToStatisticPreset();

            var user1Statistics = getStatistic(1, user1, statPreset);
            context.Add(user1Statistics);

            var user2Statistics = getStatistic(2, user2, statPreset);
            context.Add(user2Statistics);

            await context.SaveChangesAsync();

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(dtp => dtp.UtcNow()).Returns(new DateTime(2020, 11, 11));

            var leaderboardService = new LeaderboardService(context, dateTimeProvider.Object);

            // Act
            var result = await leaderboardService.GetLeaderboardEntry(user1, inputPreset);

            // Assert
            Assert.Equal(new LeaderboardEntry(id: user1.Id, name: user1.Name, rank: 1, score: 10), result);
        }

        [Theory(DisplayName =
            "GetLeaderboardEntry given user and preset when user has no score returns Leaderboard Entry with rank 0")]
        [InlineData(LeaderboardPreset.Month)]
        [InlineData(LeaderboardPreset.Semester)]
        [InlineData(LeaderboardPreset.Total)]
        public async Task GetLeaderboardEntryGivenUserAndPresetWhenUserHasNoScoreReturnsLeaderboardEntryWithRank0(
            LeaderboardPreset inputPreset)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(
                    nameof(GetLeaderboardEntryGivenUserAndPresetWhenUserHasNoScoreReturnsLeaderboardEntryWithRank0) +
                    inputPreset);

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            var user1 = testuser1;
            context.Add(user1);
            await context.SaveChangesAsync();

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            var leaderboardService = new LeaderboardService(context, dateTimeProvider.Object);

            // Act
            var result = await leaderboardService.GetLeaderboardEntry(user1, inputPreset);

            // Assert
            Assert.Equal(new LeaderboardEntry(id: user1.Id, name: user1.Name, rank: 0, score: 0), result);
        }

        [Theory(DisplayName = "GetTopLeaderboardEntries given preset returns list of leaderboard entries")]
        [InlineData(LeaderboardPreset.Month)]
        [InlineData(LeaderboardPreset.Semester)]
        [InlineData(LeaderboardPreset.Total)]
        public async Task GetTopLeaderboardEntriesPresetReturnsListLeaderboardEntry(
            LeaderboardPreset inputPreset)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetTopLeaderboardEntriesPresetReturnsListLeaderboardEntry) +
                                     inputPreset);

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            var user1 = testuser1;
            context.Add(user1);

            var user2 = testuser2;
            context.Add(user2);

            var statPreset = inputPreset.ToStatisticPreset();

            var user1Statistics = getStatistic(1, user1, statPreset);
            context.Add(user1Statistics);

            var user2Statistics = getStatistic(2, user2, statPreset);
            user2Statistics.SwipeCount = 20;
            context.Add(user2Statistics);

            await context.SaveChangesAsync();

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(dtp => dtp.UtcNow()).Returns(new DateTime(2020, 11, 11));

            var leaderboardService = new LeaderboardService(context, dateTimeProvider.Object);

            // Act
            var result = await leaderboardService.GetTopLeaderboardEntries(inputPreset, 10);

            // Assert
            Assert.Equal(new List<LeaderboardEntry>
            {
                new LeaderboardEntry(id: user2.Id, name: user2.Name, rank: 1, score: 20),
                new LeaderboardEntry(id: user1.Id, name: user1.Name, rank: 2, score: 10),
            }, result.ToList());
        }

        [Theory(DisplayName = "GetTopLeaderboardEntries given preset when statistics are expired returns empty list")]
        [InlineData(LeaderboardPreset.Month)]
        [InlineData(LeaderboardPreset.Semester)]
        public async Task GetTopLeaderboardEntriesGivensPresetWhenStatisticsAreExpiredReturnsEmptyList(
            LeaderboardPreset inputPreset)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetTopLeaderboardEntriesGivensPresetWhenStatisticsAreExpiredReturnsEmptyList) +
                                     inputPreset);

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            var user1 = testuser1;
            context.Add(user1);

            var user2 = testuser2;
            context.Add(user2);

            var statPreset = inputPreset.ToStatisticPreset();

            var user1Statistics = getStatistic(1, user1, statPreset);
            context.Add(user1Statistics);

            var user2Statistics = getStatistic(2, user2, statPreset);
            context.Add(user2Statistics);

            await context.SaveChangesAsync();

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(dtp => dtp.UtcNow()).Returns(new DateTime(2021, 12, 31));

            var leaderboardService = new LeaderboardService(context, dateTimeProvider.Object);

            // Act
            var result = await leaderboardService.GetTopLeaderboardEntries(inputPreset, 10);

            // Assert
            Assert.Empty(result.ToList());
        }

        [Theory(DisplayName = "GetTopLeaderboardEntries given preset when no statistics exists returns empty list")]
        [InlineData(LeaderboardPreset.Month)]
        [InlineData(LeaderboardPreset.Semester)]
        [InlineData(LeaderboardPreset.Total)]
        public async Task GetTopLeaderboardEntriesGivenPresetWhenNoStatisticsExistsReturnsEmptyList(
            LeaderboardPreset inputPreset)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetTopLeaderboardEntriesGivenPresetWhenNoStatisticsExistsReturnsEmptyList) +
                                     inputPreset);

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            var dateTimeProvider = new Mock<IDateTimeProvider>();
            var leaderboardService = new LeaderboardService(context, dateTimeProvider.Object);

            // Act
            var result = await leaderboardService.GetTopLeaderboardEntries(inputPreset, 10);

            // Assert
            Assert.Empty(result.ToList());
        }

        [Theory(DisplayName = "GetTopLeaderboardEntries given preset does not include expired entries")]
        [InlineData(LeaderboardPreset.Month)]
        [InlineData(LeaderboardPreset.Semester)]
        public async Task GetTopLeaderboardEntriesGivenPresetDoesNotIncludeExpiredEntries(
            LeaderboardPreset inputPreset)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(GetTopLeaderboardEntriesGivenPresetDoesNotIncludeExpiredEntries) +
                                     inputPreset);

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            var user1 = testuser1;
            context.Add(user1);

            var user2 = testuser2;
            context.Add(user2);

            var statPreset = inputPreset.ToStatisticPreset();

            var user1Statistics = getStatistic(1, user1, statPreset);
            context.Add(user1Statistics);

            var user2Statistics = getStatistic(2, user2, statPreset);
            context.Add(user2Statistics);
            await context.SaveChangesAsync();

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(dtp => dtp.UtcNow()).Returns(new DateTime(2020, 11, 12));
            var leaderboardService = new LeaderboardService(context, dateTimeProvider.Object);

            // Act
            var result = await leaderboardService.GetTopLeaderboardEntries(inputPreset, 10);

            // Assert
            Assert.Empty(result.ToList());
        }
    }
}