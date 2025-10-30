using System;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Tests.Unit;

public abstract class BaseUnitTests : IDisposable, IAsyncDisposable
{
    protected readonly CoffeeCardContext InitialContext;
    protected readonly CoffeeCardContext AssertionContext;

    protected BaseUnitTests()
    {
        var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
            .EnableSensitiveDataLogging()
            .UseInMemoryDatabase(Guid.NewGuid().ToString());

        var databaseSettings = new DatabaseSettings { SchemaName = "test" };
        var environmentSettings = new EnvironmentSettings()
        {
            DeploymentUrl = "test",
            EnvironmentType = EnvironmentType.Test,
            MinAppVersion = "2.1.0",
        };

        InitialContext = new CoffeeCardContext(
            builder.Options,
            databaseSettings,
            environmentSettings
        );
        AssertionContext = new CoffeeCardContext(
            builder.Options,
            databaseSettings,
            environmentSettings
        );

        // Set the random seed used for generation of data in the builders
        // This ensures our tests are deterministic within a specific version of the code
        var seed = new Random(42);
        Bogus.Randomizer.Seed = seed;
    }

    public void Dispose()
    {
        InitialContext?.Dispose();
        AssertionContext?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (InitialContext != null)
            await InitialContext.DisposeAsync();
        if (AssertionContext != null)
            await AssertionContext.DisposeAsync();
    }
}
