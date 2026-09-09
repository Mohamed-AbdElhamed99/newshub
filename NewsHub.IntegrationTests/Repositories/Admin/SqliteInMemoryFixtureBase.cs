using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NewsHub.Infrastructure.Data;

namespace NewsHub.IntegrationTests.Repositories.Admin;

// Shared base for Admin repository integration tests: opens one SQLite
// in-memory connection per test instance (kept alive for the fixture's
// lifetime), exposes a working Context for the repository under test, and
// CreateAssertionContext() for reading back state with a clean change
// tracker.
public abstract class SqliteInMemoryFixtureBase : IDisposable
{
    private readonly SqliteConnection _connection;

    public NewsHubDbContext Context { get; }

    protected SqliteInMemoryFixtureBase()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        Context = new NewsHubDbContext(BuildOptions());
        Context.Database.EnsureCreated();
    }

    private DbContextOptions<NewsHubDbContext> BuildOptions() =>
        new DbContextOptionsBuilder<NewsHubDbContext>()
            .UseSqlite(_connection)
            .Options;

    public NewsHubDbContext CreateAssertionContext() => new(BuildOptions());

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}