using MobileMonitoringBackend.Contracts.DTOs.ModelsToDTOs;

namespace MobileMonitoringBackend.Contracts.DTOs.Responses.Room;

public class GetRecordingsResponse
{
    public ICollection<RecordingDTO> Recordings { get; set; }
}