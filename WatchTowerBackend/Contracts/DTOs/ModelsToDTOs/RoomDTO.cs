namespace WatchTowerBackend.Contracts.DTOs.ModelsToDTOs
{
    // TODO - maybe some inheritance with models without passwords???
    public class RoomDTO
    {
        public string RoomName { get; set; }
        public IEnumerable<CameraDTO> Cameras { get; set; } = new List<CameraDTO>();
    }
}