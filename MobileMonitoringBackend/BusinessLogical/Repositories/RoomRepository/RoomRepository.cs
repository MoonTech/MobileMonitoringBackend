using Microsoft.EntityFrameworkCore;
using MobileMonitoringBackend.BusinessLogical.Utils;
using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions;
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
            throw new CouldNotSaveChangesException("Could not save changes in database");
        }
        catch
        {
            throw new ObjectAlreadyExistsInDbException("Such an user already exists in database");
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
            throw new WrongPasswordException("Provided password is wrong");
        }
        throw new ObjectDoesNotExistInDbException("Room with such a name does not exist");
    }


    public RoomModel GetRoomByName(string name)
    {
        var result = context.Rooms.Include(room => room.Cameras)
            .SingleOrDefault(room => room.RoomName == name);
        if (result is not null)
        {
            return result;
        }
        throw new ObjectDoesNotExistInDbException("Room of such a name does not exist");
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