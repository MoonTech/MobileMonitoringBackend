using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace WatchTowerAPI.Domain.Models;

[Table("Cameras")]
public class CameraModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public string CameraId { get; set; }
    public Guid? RoomId { get; set; }
    
    [ForeignKey("RoomId")]
    public RoomModel? Room { get; set; }
}