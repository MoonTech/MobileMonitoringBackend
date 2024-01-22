using Microsoft.EntityFrameworkCore;
using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions;
using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Camera;
using MobileMonitoringBackend.DataAccess.DbContexts;
using MobileMonitoringBackend.Domain.Models;

namespace MobileMonitoringBackend.BusinessLogical.Repositories.CameraRepository;

public class CameraRepository : BaseRepository, ICameraRepository
{
    public CameraRepository(MobileMonitoringDbContext context) : base(context) {}
    
    public CameraModel CreateCameraWithRoom(string cameraName, RoomModel room)
    {
        try
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
            throw new CouldNotSaveChangesException();
        }
        catch (Exception)
        {
            throw new CameraAlreadyExistsException
                ($"Camera {cameraName} already exists in room {room.RoomName}");
        }
    }

    public CameraModel GetCameraById(Guid id)
    {
        var result = context.Cameras.Include(camera => camera.Room)
            .SingleOrDefault(camera => camera.Id == id);
        if (result is not null)
        {
            return result;
        }
        throw new CameraDoesNotExistException($"Camera of id {id} does not exist");
    }

    public bool DeleteCamera(CameraModel camera)
    {
        context.Cameras.Remove(camera);
        return SaveChanges();
    }

    public bool AcceptCamera(CameraModel camera)
    {
        camera.AcceptationState = true;
        if (SaveChanges())
        {
            return true;
        }
        GetCameraById(camera.Id);
        throw new CameraAlreadyAcceptedException(camera.Id);
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