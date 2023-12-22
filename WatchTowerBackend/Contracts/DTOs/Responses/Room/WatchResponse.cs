using WatchTowerAPI.Domain.Models;
using WatchTowerBackend.Contracts.DTOs.ModelsToDTOs;

namespace WatchTowerAPI.Contracts.DTOs.Responses.Room;

public class WatchResponse
{
    public string RoomName { get; set; }
    public ICollection<WatchCameraDTO> ConnectedCameras { get; set; } = new List<WatchCameraDTO>();
}