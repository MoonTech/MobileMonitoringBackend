using WatchTowerAPI.Domain.Models;
using WatchTowerBackend.Contracts.DTOs.ModelsWithoutPasswords;

namespace WatchTowerAPI.Contracts.DTOs.Responses.Room;

public class WatchResponse
{
    public string RoomName { get; set; }
    public ICollection<CameraDTO> ConnectedCameras { get; set; } = new List<CameraDTO>();
}