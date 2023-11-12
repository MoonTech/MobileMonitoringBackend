using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using HostingEnvironmentExtensions = Microsoft.AspNetCore.Hosting.HostingEnvironmentExtensions;

namespace WatchTowerAPI.Domain.Models;



[Table("Rooms")]
[Index(nameof(RoomName), IsUnique = true)]
public class RoomModel
{
    [Key]
    public string RoomName { get; set; }
    
    public string? Password { get; set; }

    public ICollection<CameraModel> Cameras { get; set; }
    
    [ForeignKey("OwnerLogin")]
    public UserModel? Owner { get; set; }
    public string OwnerLogin { get; set; }
}