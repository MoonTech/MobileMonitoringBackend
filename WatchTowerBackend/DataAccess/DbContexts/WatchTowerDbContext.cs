using Microsoft.EntityFrameworkCore;
using WatchTowerBackend.Domain.Models;

namespace WatchTowerBackend.DataAccess.DbContexts
{
    public class WatchTowerDbContext : DbContext
    {
        public WatchTowerDbContext(DbContextOptions options): base(options) { }
        
        public DbSet<CameraModel> Cameras { get; set; }
        public DbSet<RoomModel> Rooms { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<RecordingModel> Recordings { get; set; }
    }
}