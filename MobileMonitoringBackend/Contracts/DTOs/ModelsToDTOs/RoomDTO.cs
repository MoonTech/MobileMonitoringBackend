namespace MobileMonitoringBackend.Contracts.DTOs.ModelsToDTOs
{
    public class RoomDTO
    {
        public string RoomName { get; set; }
        public IEnumerable<CameraDTO> Cameras { get; set; } = new List<CameraDTO>();
    }
}