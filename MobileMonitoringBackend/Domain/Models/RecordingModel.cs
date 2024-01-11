using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileMonitoringBackend.Domain.Models;

public class RecordingModel
{
    [Key]
    public string Name { get; set; }
    
    public string Url { get; set; }
    
    [ForeignKey("RoomName")]
    public RoomModel? Room { get; set; }
    public string RoomName { get; set; }
}