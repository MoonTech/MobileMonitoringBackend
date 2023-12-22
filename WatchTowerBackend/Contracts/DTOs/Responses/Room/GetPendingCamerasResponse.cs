using WatchTowerAPI.Domain.Models;
using WatchTowerBackend.Contracts.DTOs.ModelsToDTOs;

namespace WatchTowerAPI.Contracts.DTOs.Responses.Room;

public class GetPendingCamerasResponse
{
    public ICollection<CameraDTO> PendingCameras { get; set; } = new List<CameraDTO>();
}