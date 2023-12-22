using WatchTowerBackend.Contracts.DTOs.ModelsToDTOs;

namespace WatchTowerAPI.Contracts.DTOs.Responses.Room;

public class GetRecordingsResponse
{
    public ICollection<RecordingDTO> Recordings { get; set; }
}