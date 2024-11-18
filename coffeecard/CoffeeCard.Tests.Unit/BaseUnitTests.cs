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
            .UseInMemoryDatabase(Guid.NewGuid().ToString());

        var databaseSettings = new DatabaseSettings
        {
            SchemaName = "test"
        };
        var environmentSettings = new EnvironmentSettings()
            { DeploymentUrl = "test", EnvironmentType = EnvironmentType.Test, MinAppVersion = "2.1.0" };
        
        InitialContext = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
        AssertionContext = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
    }

    public void Dispose()
    {
        InitialContext?.Dispose();
        AssertionContext?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (InitialContext != null) await InitialContext.DisposeAsync();
        if (AssertionContext != null) await AssertionContext.DisposeAsync();
    }
}