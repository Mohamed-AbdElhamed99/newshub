using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NewsHub.Infrastructure.Data;

namespace NewsHub.IntegrationTests.Repositories.Site;

// Shared base for Site repository integration tests. Same pattern as
// NewsHub.IntegrationTests.Repositories.Admin.SqliteInMemoryFixtureBase —
// consider consolidating both into a single Common namespace when convenient.
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
