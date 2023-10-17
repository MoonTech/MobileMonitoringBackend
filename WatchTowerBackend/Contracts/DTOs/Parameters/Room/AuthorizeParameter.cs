namespace WatchTowerAPI.Contracts.DTOs.Parameters.Room;

public class AuthorizeParameter
{
    public Guid RoomId { get; set; }
    public string Password { get; set; }
}