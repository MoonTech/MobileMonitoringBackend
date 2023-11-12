using WatchTowerAPI.Domain.Models;

namespace WatchTowerAPI.BusinessLogical.Repositories.RoomRepository;

public interface IRoomRepository
{
    public RoomModel? CreateRoom(string name, string password, UserModel owner);
    public RoomModel? GetRoom(string name, string password);
    public RoomModel GetRoomByName(string name);
    public bool DeleteRoom(RoomModel name);
    public RoomModel GetFirstRoom(); // TODO Delete it, it is just for test
    public bool CheckRoomAndPassword(string roomName, string? password);
}