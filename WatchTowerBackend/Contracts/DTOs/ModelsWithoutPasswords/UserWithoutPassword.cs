namespace WatchTowerBackend.Contracts.DTOs.ModelsWithoutPasswords;

public class UserWithoutPassword
{
    public string Login { get; set; }
    public ICollection<RoomWithoutPassword> Rooms { get; set; }
}