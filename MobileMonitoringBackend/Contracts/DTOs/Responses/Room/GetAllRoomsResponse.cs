using MobileMonitoringBackend.Contracts.DTOs.ModelsToDTOs;

namespace MobileMonitoringBackend.Contracts.DTOs.Responses.Room;

public class GetAllRoomsResponse
{
    public ICollection<RoomDTO> Rooms { get; set; }
}