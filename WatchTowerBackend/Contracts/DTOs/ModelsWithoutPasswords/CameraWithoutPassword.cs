namespace WatchTowerBackend.Contracts.DTOs.ModelsWithoutPasswords;

public class CameraWithoutPassword
{
    public Guid Id { get; set; }
    public bool? AcceptationState { get; set; }
    public RoomWithoutPassword? Room { get; set; }
}