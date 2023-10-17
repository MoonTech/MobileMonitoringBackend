using WatchTowerAPI.DataAccess.DbContexts;
using WatchTowerAPI.Domain.Models;

namespace WatchTowerAPI.BusinessLogical.Repositories.RoomRepository;

public class RoomRepository : BaseRepository, IRoomRepository
{
    public RoomRepository(WatchTowerDbContext context) : base(context) {}

    public Guid CreateRoom(string password)
    {
        var newRoom = context.Rooms.Add(
            new RoomModel()
            {
                Password = password
            });
        if (SaveChanges())
        {
            return newRoom.Entity.RoomId;
        }
        else
        {
            // TODO Exception
            throw new Exception("Could not create a room");
        }
    }

    public RoomModel GetRoomById(Guid id)
    {
        var result = context.Rooms.Find(id);
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

    public bool CheckRoomAndPassword(Guid id, string? password)
    {
        var roomToCheck = context.Rooms.Find(id);
        if (roomToCheck?.Password == password ) // TODO ? !!!
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}