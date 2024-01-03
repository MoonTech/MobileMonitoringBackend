using WatchTowerBackend.Contracts.DTOs.ModelsToDTOs;

namespace WatchTowerBackend.Contracts.DTOs.Responses.Room;

public class GetAllRoomsResponse
{
    public ICollection<RoomDTO> Rooms { get; set; }
}