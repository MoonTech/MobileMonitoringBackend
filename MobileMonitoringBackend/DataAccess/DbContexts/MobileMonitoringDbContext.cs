using Microsoft.EntityFrameworkCore;
using MobileMonitoringBackend.Domain.Models;

namespace MobileMonitoringBackend.DataAccess.DbContexts
{
    public class MobileMonitoringDbContext : DbContext
    {
        public MobileMonitoringDbContext(DbContextOptions options): base(options) { }
        
        public DbSet<CameraModel> Cameras { get; set; }
        public DbSet<RoomModel> Rooms { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<RecordingModel> Recordings { get; set; }
    }
}