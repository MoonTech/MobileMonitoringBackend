using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WatchTowerBackend.DataAccess.DbContexts;

namespace WatchTowerBackendTests.Utils;

public static class RepositoryMockTest
{
    public static WatchTowerDbContext CreateMockDbContext()
    {
        var _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        var _contextOptions = new DbContextOptionsBuilder<WatchTowerDbContext>()
            .UseSqlite(_connection)
            .Options;

        var mockDbContext = new WatchTowerDbContext(_contextOptions);
        mockDbContext.Database.EnsureDeleted();
        mockDbContext.Database.EnsureCreated();

        return mockDbContext;
    }
}