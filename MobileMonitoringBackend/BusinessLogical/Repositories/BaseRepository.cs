using MobileMonitoringBackend.DataAccess.DbContexts;

namespace MobileMonitoringBackend.BusinessLogical.Repositories;

public abstract class BaseRepository
{
    protected readonly MobileMonitoringDbContext context;
    
    protected bool SaveChanges()
    {
        var saved = context.SaveChanges();
        return saved > 0;
    }

    public BaseRepository(MobileMonitoringDbContext context)
    {
        this.context = context;
    }
}