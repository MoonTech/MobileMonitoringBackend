using WatchTowerAPI.Domain.Models;

namespace WatchTowerAPI.BusinessLogical.Repositories.RoomRepository;

public interface IRoomRepository
{
    public RoomModel CreateRoom(string name, string password, UserModel owner);
    public RoomModel GetRoomByName(string name);
    public RoomModel GetFirstRoom(); // TODO Delete it, it is just for test
    public bool CheckRoomAndPassword(string roomName, string? password);
}