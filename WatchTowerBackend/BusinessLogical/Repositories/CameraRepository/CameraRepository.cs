using Microsoft.EntityFrameworkCore;
using WatchTowerAPI.DataAccess.DbContexts;
using WatchTowerAPI.Domain.Models;

namespace WatchTowerAPI.BusinessLogical.Repositories.CameraRepository;

public class CameraRepository : BaseRepository, ICameraRepository
{
    public CameraRepository(WatchTowerDbContext context) : base(context) {}
    
    public CameraModel? CreateCameraWithRoom(string cameraName, RoomModel room)
    {
        var roomEntity = context.Cameras.Add(new CameraModel()
        {
            CameraName = cameraName,
            AcceptationState = false,
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

    public CameraModel? GetCameraById(Guid id)
    {
        var result = context.Cameras.Include(camera => camera.Room)
            .SingleOrDefault(camera => camera.Id == id);
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

    public bool DeleteCamera(CameraModel camera)
    {
        context.Cameras.Remove(camera);
        return SaveChanges();
    }

    public bool AcceptCamera(CameraModel camera)
    {
        camera.AcceptationState = true;
        return SaveChanges();
    }

    public bool RejectCamera(CameraModel camera)
    {
        camera.AcceptationState = false;
        return SaveChanges();
    }

    public bool AssignNewRoom(Guid id, RoomModel room)
    {
        var camera = GetCameraById(id);
        camera.Room = room;
        return SaveChanges();
    }
}