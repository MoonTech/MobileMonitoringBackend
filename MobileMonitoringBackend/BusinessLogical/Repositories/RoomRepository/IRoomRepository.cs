using MobileMonitoringBackend.Domain.Models;

namespace MobileMonitoringBackend.BusinessLogical.Repositories.RoomRepository;

public interface IRoomRepository
{
    public RoomModel CreateRoom(string name, string password, UserModel owner);
    public RoomModel GetRoom(string name, string password);
    public RoomModel GetRoomByName(string name);
    public bool DeleteRoom(RoomModel name);
    public bool CheckRoomAndPassword(string roomName, string? password);
}