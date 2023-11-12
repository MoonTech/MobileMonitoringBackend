using Microsoft.EntityFrameworkCore;
using WatchTowerAPI.DataAccess.DbContexts;
using WatchTowerAPI.Domain.Models;
using WatchTowerBackend.Contracts.DTOs.ModelsWithoutPasswords;

namespace WatchTowerAPI.BusinessLogical.Repositories.RoomRepository;

public class RoomRepository : BaseRepository, IRoomRepository
{
    public RoomRepository(WatchTowerDbContext context) : base(context) {}

    public RoomModel? CreateRoom(string name, string password, UserModel owner)
    {
        var newRoom = context.Rooms.Add(
            new RoomModel()
            {
                RoomName = name,
                Password = password,
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
        var result = context.Rooms.Include(room => room.Cameras).SingleOrDefault(room => room.RoomName == name
                                                           && room.Password == password);
        if (result is not null)
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

    // TODO Check whether it deletes cameras as well
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
        if (roomToCheck?.Password == password || roomToCheck?.Password == null) // TODO ? !!!
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}