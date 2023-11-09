using WatchTowerAPI.DataAccess.DbContexts;
using WatchTowerAPI.Domain.Models;

namespace WatchTowerAPI.BusinessLogical.Repositories.RoomRepository;

public class RoomRepository : BaseRepository, IRoomRepository
{
    public RoomRepository(WatchTowerDbContext context) : base(context) {}

    public RoomModel CreateRoom(string name, string password, UserModel owner)
    {
        var newRoom = context.Rooms.Add(
            new RoomModel()
            {
                RoomName = name,
                Password = password,
                Owner = owner
            });
        if (SaveChanges())
        {
            return newRoom.Entity;
        }
        else
        {
            // TODO Exception
            throw new Exception("Could not create a room");
        }
    }

    public RoomModel GetRoomByName(string name)
    {
        var result = context.Rooms.Find(name);
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