using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WatchTowerBackend.Domain.Models;



[Table("Rooms")]
[Index(nameof(RoomName), IsUnique = true)]
public class RoomModel
{
    [Key]
    public string RoomName { get; set; }
    
    public string? Password { get; set; }

    public ICollection<CameraModel> Cameras { get; set; }
    public ICollection<RecordingModel> Recordings { get; set; }

    [ForeignKey("OwnerLogin")]
    public UserModel? Owner { get; set; }
    public string OwnerLogin { get; set; }
}