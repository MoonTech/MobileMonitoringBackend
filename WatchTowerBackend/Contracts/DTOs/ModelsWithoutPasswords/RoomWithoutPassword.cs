namespace WatchTowerBackend.Contracts.DTOs.ModelsWithoutPasswords;

public class RoomWithoutPassword
{
    public string RoomName { get; set; }
    public ICollection<CameraWithoutPassword> Cameras { get; set; }
    public UserWithoutPassword? Owner { get; set; }
}