using Microsoft.EntityFrameworkCore;
using MobileMonitoringBackend.BusinessLogical.Utils;
using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions;
using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Room;
using MobileMonitoringBackend.DataAccess.DbContexts;
using MobileMonitoringBackend.Domain.Models;

namespace MobileMonitoringBackend.BusinessLogical.Repositories.RoomRepository;

public class RoomRepository : BaseRepository, IRoomRepository
{
    public RoomRepository(MobileMonitoringDbContext context) : base(context) {}

    public RoomModel CreateRoom(string name, string password, UserModel owner)
    {
        try
        {
            var newRoom = new RoomModel()
                {
                    RoomName = name,
                    Password = PasswordHash.HashPassword(password),
                    Owner = owner
                };
            context.Rooms.Add(newRoom);
            if (SaveChanges())
            {
                return newRoom;
            }
            throw new CouldNotSaveChangesException();
        }
        catch
        {
            throw new RoomAlreadyExistsException(name);
        }
    }

    public RoomModel GetRoom(string name, string password)
    {
        var result = context.Rooms.Include(room => room.Cameras)
            .SingleOrDefault(room => room.RoomName == name);
        if (result is not null)
        {
            if (PasswordHash.VerifyPassword(password, result.Password))
            {
                return result;
            }
            throw new RoomPasswordWrongException(name);
        }
        throw new RoomDoesNotExistException(name);
    }


    public RoomModel GetRoomByName(string name)
    {
        var result = context.Rooms.Include(room => room.Cameras)
                .SingleOrDefault(room => room.RoomName == name);
        if (result is not null)
        {
            return result;
        }
        throw new RoomDoesNotExistException(name);
    }
    
    public bool DeleteRoom(RoomModel roomToRemove)
    {
        context.Rooms.Remove(roomToRemove);
        return SaveChanges();
    }

    public bool CheckRoomAndPassword(string roomName, string? password)
    {
        var roomToCheck = context.Rooms.Find(roomName);
        if (roomToCheck is not null)
        {
            return PasswordHash.VerifyPassword(password, roomToCheck.Password);
        }
        return false;
    }
    
}