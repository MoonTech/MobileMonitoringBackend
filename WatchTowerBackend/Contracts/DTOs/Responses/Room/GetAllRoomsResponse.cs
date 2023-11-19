using WatchTowerAPI.Domain.Models;
using WatchTowerBackend.Contracts.DTOs.ModelsWithoutPasswords;

namespace WatchTowerAPI.Contracts.DTOs.Responses.Room;

public class GetAllRoomsResponse
{
    public ICollection<RoomDTO> Rooms { get; set; }
}