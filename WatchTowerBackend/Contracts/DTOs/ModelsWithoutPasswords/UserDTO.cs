using WatchTowerAPI.Domain.Models;

namespace WatchTowerBackend.Contracts.DTOs.ModelsWithoutPasswords;

public class UserDTO
{
    public string Login { get; set; }
    public IEnumerable<RoomDTO> Rooms { get; set; }
}