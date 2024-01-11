namespace MobileMonitoringBackend.Contracts.DTOs.ModelsToDTOs
{
    public class UserDTO
    {
        public string Login { get; set; }
        public IEnumerable<RoomDTO> Rooms { get; set; } = new List<RoomDTO>();
    }
}