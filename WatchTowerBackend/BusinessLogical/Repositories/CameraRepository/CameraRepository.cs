using WatchTowerAPI.DataAccess.DbContexts;
using WatchTowerAPI.Domain.Models;

namespace WatchTowerAPI.BusinessLogical.Repositories.CameraRepository;

public class CameraRepository : BaseRepository, ICameraRepository
{
    public CameraRepository(WatchTowerDbContext context) : base(context) {}
    
    public bool CreateCameraWithRoom(string cameraId, RoomModel room)
    {
        context.Cameras.Add(new CameraModel()
        {
            CameraId = cameraId,
            Room = room
        });
        return SaveChanges();
    }

    public CameraModel GetCameraById(string id)
    {
        var result = context.Cameras.Find(id);
        if (result is not null)
        {
            return result;
        }
        // TODO Create own exception type
        else
        {
            throw new Exception("Camera does not exist");
        }
    }

    public bool AssignNewRoom(string cameraId, RoomModel room)
    {
        var camera = GetCameraById(cameraId);
        camera.Room = room;
        return SaveChanges();
    }
}