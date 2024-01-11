using Microsoft.EntityFrameworkCore;
using MobileMonitoringBackend.BusinessLogical.Utils;
using MobileMonitoringBackend.DataAccess.DbContexts;
using MobileMonitoringBackend.Domain.Models;

namespace MobileMonitoringBackend.BusinessLogical.Repositories.RoomRepository;

public class RoomRepository : BaseRepository, IRoomRepository
{
    public RoomRepository(MobileMonitoringDbContext context) : base(context) {}

    public RoomModel? CreateRoom(string name, string password, UserModel owner)
    {
        var newRoom = context.Rooms.Add(
            new RoomModel()
            {
                RoomName = name,
                Password = PasswordHash.HashPassword(password),
                Owner = owner
            })
            .Entity;
        if (SaveChanges())
        {
            return newRoom;
        }
        else
        {
            return null;
        }
    }

    public RoomModel? GetRoom(string name, string password)
    {
        var result = context.Rooms.Include(room => room.Cameras)
            .SingleOrDefault(room => room.RoomName == name);
        if (result is not null && PasswordHash.VerifyPassword(password, result.Password))
        {
            return result;
        }
        // TODO Create own exception type
        else
        {
            throw new Exception("Room does not exist or password is incorrect");
        }
    }

    public RoomModel GetRoomByName(string name)
    {
        var result = context.Rooms.Include(room => room.Cameras)
            .SingleOrDefault(room => room.RoomName == name);
        if (result is not null)
        {
            return result;
        }
        // TODO Create own exception type
        else
        {
            throw new Exception("Room does not exist");
        }
    }
    
    public bool DeleteRoom(RoomModel roomToRemove)
    {
        context.Rooms.Remove(roomToRemove);
        return SaveChanges();
    }

    public RoomModel GetFirstRoom()
    {
        return context.Rooms.First();
    }

    public bool CheckRoomAndPassword(string roomName, string? password)
    {
        var roomToCheck = context.Rooms.Find(roomName);
        return PasswordHash.VerifyPassword(password, roomToCheck.Password);
    }


}