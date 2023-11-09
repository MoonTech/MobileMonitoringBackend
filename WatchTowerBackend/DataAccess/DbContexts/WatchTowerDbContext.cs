using WatchTowerAPI.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace WatchTowerAPI.DataAccess.DbContexts
{
    public class WatchTowerDbContext : DbContext
    {
        public WatchTowerDbContext(DbContextOptions options): base(options) { }
        
        public DbSet<CameraModel> Cameras { get; set; }
        public DbSet<RoomModel> Rooms { get; set; }
        public DbSet<UserModel> Users { get; set; }
    }
}