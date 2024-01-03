using WatchTowerBackend.Contracts.DTOs.ModelsToDTOs;

namespace WatchTowerBackend.Contracts.DTOs.Responses.Room;

public class GetPendingCamerasResponse
{
    public ICollection<CameraDTO> PendingCameras { get; set; } = new List<CameraDTO>();
}