using WatchTowerAPI.Domain.Models;

namespace WatchTowerAPI.BusinessLogical.Repositories.RoomRepository;

public interface IRoomRepository
{
    public Guid CreateRoom(string password);
    public RoomModel GetRoomById(Guid id);
    public bool CheckRoomAndPassword(Guid id, string? password);
}