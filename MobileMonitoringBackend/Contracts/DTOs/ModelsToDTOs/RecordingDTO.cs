using MobileMonitoringBackend.Domain.Models;

namespace MobileMonitoringBackend.Contracts.DTOs.ModelsToDTOs;

public class RecordingDTO
{
    public string Name { get; set; }
    public string Url { get; set; }
    
    public static implicit operator RecordingDTO(RecordingModel recording)
    {
        return new ()
        {
            Name = recording.Name,
            Url = recording.Url
        };
    }
}