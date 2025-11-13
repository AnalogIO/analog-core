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
        [Theory(
            DisplayName = "GetLeaderboardEntry given user and preset returns Leaderboard Entry"
        )]
        [InlineData(LeaderboardPreset.Month)]
        [InlineData(LeaderboardPreset.Semester)]
        [InlineData(LeaderboardPreset.Total)]
        public async Task GetLeaderboardEntryGivenUserAndPresetReturnsLeaderboardEntry(
            LeaderboardPreset inputPreset
        )
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(GetLeaderboardEntryGivenUserAndPresetReturnsLeaderboardEntry) + inputPreset
            );

            var databaseSettings = new DatabaseSettings { SchemaName = "test" };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test,
            };

            await using var context = new CoffeeCardContext(
                builder.Options,
                databaseSettings,
                environmentSettings
            );
            var user1 = new User
            {
                Id = 1,
                Name = "User1",
                Email = "email@email.test",
                Password = "password",
                Salt = "salt",
                DateCreated = new DateTime(year: 2020, month: 11, day: 11),
                IsVerified = true,
                PrivacyActivated = false,
                UserGroup = UserGroup.Customer,
                UserState = UserState.Active,
            };
            context.Add(user1);

            var user2 = new User
            {
                Id = 2,
                Name = "User2",
                Email = "email@email.test",
                Password = "password",
                Salt = "salt",
                DateCreated = new DateTime(year: 2020, month: 11, day: 11),
                IsVerified = true,
                PrivacyActivated = false,
                UserGroup = UserGroup.Customer,
                UserState = UserState.Active,
            };
            context.Add(user2);

            var user1Statistics = new Statistic
            {
                Id = 1,
                Preset = inputPreset.ToStatisticPreset(),
                SwipeCount = 10,
                LastSwipe = new DateTime(year: 2020, month: 11, day: 11),
                ExpiryDate = new DateTime(year: 2020, month: 11, day: 11),
                User = user1,
            };
            context.Add(user1Statistics);

            var user2Statistics = new Statistic
            {
                Id = 2,
                Preset = inputPreset.ToStatisticPreset(),
                SwipeCount = 20,
                LastSwipe = new DateTime(year: 2020, month: 11, day: 11),
                ExpiryDate = new DateTime(year: 2020, month: 11, day: 11),
                User = user2,
            };
            context.Add(user2Statistics);

            await context.SaveChangesAsync();

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(dtp => dtp.UtcNow()).Returns(new DateTime(2020, 11, 11));

            var leaderboardService = new LeaderboardService(context, dateTimeProvider.Object);

            // Act
            var result = await leaderboardService.GetLeaderboardEntry(user1, inputPreset);

            // Assert
            Assert.Equal(
                new LeaderboardEntry
                {
                    Id = user1.Id,
                    Name = user1.Name,
                    Rank = 2,
                    Score = 10,
                },
                result
            );
        }

        [Theory(
            DisplayName = "GetLeaderboardEntry given user and preset when user has no score returns Leaderboard Entry with rank 0"
        )]
        [InlineData(LeaderboardPreset.Month)]
        [InlineData(LeaderboardPreset.Semester)]
        [InlineData(LeaderboardPreset.Total)]
        public async Task GetLeaderboardEntryGivenUserAndPresetWhenUserHasNoScoreReturnsLeaderboardEntryWithRank0(
            LeaderboardPreset inputPreset
        )
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(
                    GetLeaderboardEntryGivenUserAndPresetWhenUserHasNoScoreReturnsLeaderboardEntryWithRank0
                ) + inputPreset
            );

            var databaseSettings = new DatabaseSettings { SchemaName = "test" };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test,
            };

            await using var context = new CoffeeCardContext(
                builder.Options,
                databaseSettings,
                environmentSettings
            );
            var user1 = new User
            {
                Id = 1,
                Name = "User1",
                Email = "email@email.test",
                Password = "password",
                Salt = "salt",
                DateCreated = new DateTime(year: 2020, month: 11, day: 11),
                IsVerified = true,
                PrivacyActivated = false,
                UserGroup = UserGroup.Customer,
                UserState = UserState.Active,
            };
            context.Add(user1);
            await context.SaveChangesAsync();

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            var leaderboardService = new LeaderboardService(context, dateTimeProvider.Object);

            // Act
            var result = await leaderboardService.GetLeaderboardEntry(user1, inputPreset);

            // Assert
            Assert.Equal(
                new LeaderboardEntry
                {
                    Id = user1.Id,
                    Name = user1.Name,
                    Rank = 0,
                    Score = 0,
                },
                result
            );
        }

        [Theory(
            DisplayName = "GetTopLeaderboardEntries given preset returns list of leaderboard entries"
        )]
        [InlineData(LeaderboardPreset.Month)]
        [InlineData(LeaderboardPreset.Semester)]
        [InlineData(LeaderboardPreset.Total)]
        public async Task GetTopLeaderboardEntriesPresetReturnsListLeaderboardEntry(
            LeaderboardPreset inputPreset
        )
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(GetTopLeaderboardEntriesPresetReturnsListLeaderboardEntry) + inputPreset
            );

            var databaseSettings = new DatabaseSettings { SchemaName = "test" };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test,
            };

            await using var context = new CoffeeCardContext(
                builder.Options,
                databaseSettings,
                environmentSettings
            );
            var user1 = new User
            {
                Id = 1,
                Name = "User1",
                Email = "email@email.test",
                Password = "password",
                Salt = "salt",
                DateCreated = new DateTime(year: 2020, month: 11, day: 11),
                IsVerified = true,
                PrivacyActivated = false,
                UserGroup = UserGroup.Customer,
                UserState = UserState.Active,
            };
            context.Add(user1);

            var user2 = new User
            {
                Id = 2,
                Name = "User2",
                Email = "email@email.test",
                Password = "password",
                Salt = "salt",
                DateCreated = new DateTime(year: 2020, month: 11, day: 11),
                IsVerified = true,
                PrivacyActivated = false,
                UserGroup = UserGroup.Customer,
                UserState = UserState.Active,
            };
            context.Add(user2);

            var user1Statistics = new Statistic
            {
                Id = 1,
                Preset = inputPreset.ToStatisticPreset(),
                SwipeCount = 10,
                LastSwipe = new DateTime(year: 2020, month: 11, day: 11),
                ExpiryDate = new DateTime(year: 2020, month: 11, day: 11),
                User = user1,
            };
            context.Add(user1Statistics);

            var user2Statistics = new Statistic
            {
                Id = 2,
                Preset = inputPreset.ToStatisticPreset(),
                SwipeCount = 20,
                LastSwipe = new DateTime(year: 2020, month: 11, day: 11),
                ExpiryDate = new DateTime(year: 2020, month: 11, day: 11),
                User = user2,
            };
            context.Add(user2Statistics);

            await context.SaveChangesAsync();

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(dtp => dtp.UtcNow()).Returns(new DateTime(2020, 11, 11));

            var leaderboardService = new LeaderboardService(context, dateTimeProvider.Object);

            // Act
            var result = await leaderboardService.GetTopLeaderboardEntries(inputPreset, 10);

            // Assert
            Assert.Equal(
                new List<LeaderboardEntry>
                {
                    new LeaderboardEntry
                    {
                        Id = user2.Id,
                        Name = user2.Name,
                        Rank = 1,
                        Score = 20,
                    },
                    new LeaderboardEntry
                    {
                        Id = user1.Id,
                        Name = user1.Name,
                        Rank = 2,
                        Score = 10,
                    },
                },
                result.ToList()
            );
        }

        [Theory(
            DisplayName = "GetTopLeaderboardEntries given preset when statistics are expired returns empty list"
        )]
        [InlineData(LeaderboardPreset.Month)]
        [InlineData(LeaderboardPreset.Semester)]
        public async Task GetTopLeaderboardEntriesGivensPresetWhenStatisticsAreExpiredReturnsEmptyList(
            LeaderboardPreset inputPreset
        )
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(GetTopLeaderboardEntriesGivensPresetWhenStatisticsAreExpiredReturnsEmptyList)
                    + inputPreset
            );

            var databaseSettings = new DatabaseSettings { SchemaName = "test" };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test,
            };

            await using var context = new CoffeeCardContext(
                builder.Options,
                databaseSettings,
                environmentSettings
            );
            var user1 = new User
            {
                Id = 1,
                Name = "User1",
                Email = "email@email.test",
                Password = "password",
                Salt = "salt",
                DateCreated = new DateTime(year: 2020, month: 11, day: 11),
                IsVerified = true,
                PrivacyActivated = false,
                UserGroup = UserGroup.Customer,
                UserState = UserState.Active,
            };
            context.Add(user1);

            var user2 = new User
            {
                Id = 2,
                Name = "User2",
                Email = "email@email.test",
                Password = "password",
                Salt = "salt",
                DateCreated = new DateTime(year: 2020, month: 11, day: 11),
                IsVerified = true,
                PrivacyActivated = false,
                UserGroup = UserGroup.Customer,
                UserState = UserState.Active,
            };
            context.Add(user2);

            var user1Statistics = new Statistic
            {
                Id = 1,
                Preset = inputPreset.ToStatisticPreset(),
                SwipeCount = 10,
                LastSwipe = new DateTime(year: 2020, month: 11, day: 11),
                ExpiryDate = new DateTime(year: 2020, month: 11, day: 11),
                User = user1,
            };
            context.Add(user1Statistics);

            var user2Statistics = new Statistic
            {
                Id = 2,
                Preset = inputPreset.ToStatisticPreset(),
                SwipeCount = 20,
                LastSwipe = new DateTime(year: 2020, month: 11, day: 11),
                ExpiryDate = new DateTime(year: 2020, month: 11, day: 11),
                User = user2,
            };
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

        [Theory(
            DisplayName = "GetTopLeaderboardEntries given preset when no statistics exists returns empty list"
        )]
        [InlineData(LeaderboardPreset.Month)]
        [InlineData(LeaderboardPreset.Semester)]
        [InlineData(LeaderboardPreset.Total)]
        public async Task GetTopLeaderboardEntriesGivenPresetWhenNoStatisticsExistsReturnsEmptyList(
            LeaderboardPreset inputPreset
        )
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(GetTopLeaderboardEntriesGivenPresetWhenNoStatisticsExistsReturnsEmptyList)
                    + inputPreset
            );

            var databaseSettings = new DatabaseSettings { SchemaName = "test" };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test,
            };

            await using var context = new CoffeeCardContext(
                builder.Options,
                databaseSettings,
                environmentSettings
            );
            var dateTimeProvider = new Mock<IDateTimeProvider>();
            var leaderboardService = new LeaderboardService(context, dateTimeProvider.Object);

            // Act
            var result = await leaderboardService.GetTopLeaderboardEntries(inputPreset, 10);

            // Assert
            Assert.Empty(result.ToList());
        }

        [Theory(
            DisplayName = "GetTopLeaderboardEntries given preset does not include expired entries"
        )]
        [InlineData(LeaderboardPreset.Month)]
        [InlineData(LeaderboardPreset.Semester)]
        public async Task GetTopLeaderboardEntriesGivenPresetDoesNotIncludeExpiredEntries(
            LeaderboardPreset inputPreset
        )
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>().UseInMemoryDatabase(
                nameof(GetTopLeaderboardEntriesGivenPresetDoesNotIncludeExpiredEntries)
                    + inputPreset
            );

            var databaseSettings = new DatabaseSettings { SchemaName = "test" };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test,
            };

            await using var context = new CoffeeCardContext(
                builder.Options,
                databaseSettings,
                environmentSettings
            );
            var user1 = new User
            {
                Id = 1,
                Name = "User1",
                Email = "email@email.test",
                Password = "password",
                Salt = "salt",
                DateCreated = new DateTime(year: 2020, month: 11, day: 11),
                IsVerified = true,
                PrivacyActivated = false,
                UserGroup = UserGroup.Customer,
                UserState = UserState.Active,
            };
            context.Add(user1);

            var user2 = new User
            {
                Id = 2,
                Name = "User2",
                Email = "email@email.test",
                Password = "password",
                Salt = "salt",
                DateCreated = new DateTime(year: 2020, month: 11, day: 11),
                IsVerified = true,
                PrivacyActivated = false,
                UserGroup = UserGroup.Customer,
                UserState = UserState.Active,
            };
            context.Add(user2);

            var user1Statistics = new Statistic
            {
                Id = 1,
                Preset = inputPreset.ToStatisticPreset(),
                SwipeCount = 10,
                LastSwipe = new DateTime(year: 2000, month: 11, day: 11),
                ExpiryDate = new DateTime(year: 2000, month: 11, day: 11),
                User = user1,
            };
            context.Add(user1Statistics);

            var user2Statistics = new Statistic
            {
                Id = 2,
                Preset = inputPreset.ToStatisticPreset(),
                SwipeCount = 20,
                LastSwipe = new DateTime(year: 2000, month: 11, day: 11),
                ExpiryDate = new DateTime(year: 2000, month: 11, day: 11),
                User = user2,
            };
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
