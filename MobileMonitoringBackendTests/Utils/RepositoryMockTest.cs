using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MobileMonitoringBackend.DataAccess.DbContexts;

namespace MobileMonitoringBackendTests.Utils;

public static class RepositoryMockTest
{
    public static MobileMonitoringDbContext CreateMockDbContext()
    {
        var _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        var _contextOptions = new DbContextOptionsBuilder<MobileMonitoringDbContext>()
            .UseSqlite(_connection)
            .Options;

        var mockDbContext = new MobileMonitoringDbContext(_contextOptions);
        mockDbContext.Database.EnsureDeleted();
        mockDbContext.Database.EnsureCreated();

        return mockDbContext;
    }
}