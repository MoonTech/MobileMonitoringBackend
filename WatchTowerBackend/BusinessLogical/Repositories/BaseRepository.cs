using WatchTowerAPI.DataAccess.DbContexts;

namespace WatchTowerAPI.BusinessLogical.Repositories;

public abstract class BaseRepository
{
    protected readonly WatchTowerDbContext context;
    
    protected bool SaveChanges()
    {
        var saved = context.SaveChanges();
        return saved > 0;
    }

    public BaseRepository(WatchTowerDbContext context)
    {
        this.context = context;
    }
}