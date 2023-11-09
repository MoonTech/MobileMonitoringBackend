using WatchTowerAPI.DataAccess.DbContexts;
using WatchTowerAPI.Domain.Models;

namespace WatchTowerAPI.BusinessLogical.Repositories.CameraRepository;

public class CameraRepository : BaseRepository, ICameraRepository
{
    public CameraRepository(WatchTowerDbContext context) : base(context) {}
    
    public CameraModel? CreateCameraWithRoom(RoomModel room)
    {
        var roomEntity = context.Cameras.Add(new CameraModel()
        {
            Room = room
        });
        var newCamera = roomEntity.Entity;
        if (SaveChanges())
        {
            return newCamera;
        }
        else
        {
            return null;
        }
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

    public bool AssignNewRoom(string cameraName, RoomModel room)
    {
        var camera = GetCameraById(cameraName);
        camera.Room = room;
        return SaveChanges();
    }
}