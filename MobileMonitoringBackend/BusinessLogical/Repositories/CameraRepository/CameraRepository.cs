using Microsoft.EntityFrameworkCore;
using MobileMonitoringBackend.DataAccess.DbContexts;
using MobileMonitoringBackend.Domain.Models;

namespace MobileMonitoringBackend.BusinessLogical.Repositories.CameraRepository;

public class CameraRepository : BaseRepository, ICameraRepository
{
    public CameraRepository(MobileMonitoringDbContext context) : base(context) {}
    
    public CameraModel? CreateCameraWithRoom(string cameraName, RoomModel room)
    {
        var roomEntity = context.Cameras.Add(new CameraModel()
        {
            CameraName = cameraName,
            CameraToken = CreateCameraToken(10),
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
    

    private string CreateCameraToken(int length)
    {
        
        Random random = new Random();
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        bool foundUniqueToken = false;
        string token;
        do
        {
            token = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            if (context.Cameras.SingleOrDefault(c => c.CameraToken == token) is null)
            {
                foundUniqueToken = true;
            }
        } while (!foundUniqueToken);
        return token;
    }
}