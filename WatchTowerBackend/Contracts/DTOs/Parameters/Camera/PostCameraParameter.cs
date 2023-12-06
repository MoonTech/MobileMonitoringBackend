namespace WatchTowerAPI.Contracts.DTOs.Parameters.Camera;

public class PostCameraParameter
{
    public string CameraName { get; set; }
    public string RoomName { get; set; }
    public string? Password { get; set; }
}