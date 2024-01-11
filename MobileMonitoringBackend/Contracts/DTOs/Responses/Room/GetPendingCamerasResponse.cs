using MobileMonitoringBackend.Contracts.DTOs.ModelsToDTOs;

namespace MobileMonitoringBackend.Contracts.DTOs.Responses.Room;

public class GetPendingCamerasResponse
{
    public ICollection<CameraDTO> PendingCameras { get; set; } = new List<CameraDTO>();
}