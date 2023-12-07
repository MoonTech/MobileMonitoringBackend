using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace WatchTowerAPI.Domain.Models;

[Table("Cameras")]
[Index(nameof(RoomName),nameof(CameraName), IsUnique = true)]
public class CameraModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public Guid Id { get; set; }
    public string CameraName { get; set; }
    public string CameraToken { get; set; }
    public bool AcceptationState { get; set; }

    [ForeignKey("RoomName")]
    public RoomModel? Room { get; set; }
    public string RoomName { get; set; }
}