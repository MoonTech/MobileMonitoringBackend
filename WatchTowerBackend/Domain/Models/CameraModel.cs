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
    public Guid Id { get; set; }
    public bool? AcceptationState { get; set; }

    [ForeignKey("RoomName")]
    public RoomModel? Room { get; set; }
    public string RoomName { get; set; }
}