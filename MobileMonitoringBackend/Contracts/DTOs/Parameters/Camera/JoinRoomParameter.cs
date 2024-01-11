namespace MobileMonitoringBackend.Contracts.DTOs.Parameters.Camera;

public class JoinRoomParameter
{
    public string CameraId { get; set; }
    public Guid RoomId { get; set; }
}