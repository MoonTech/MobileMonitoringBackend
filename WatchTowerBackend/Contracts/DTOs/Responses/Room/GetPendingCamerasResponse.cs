using WatchTowerAPI.Domain.Models;
using WatchTowerBackend.Contracts.DTOs.ModelsWithoutPasswords;

namespace WatchTowerAPI.Contracts.DTOs.Responses.Room;

public class GetPendingCamerasResponse
{
    public List<CameraWithoutPassword> PendingCameras { get; set; }
}