using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace WatchTowerAPI.Domain.Models;

[Table("Rooms")]
public class RoomModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid RoomId { get; set; }
    
    public string? Password { get; set; }
}