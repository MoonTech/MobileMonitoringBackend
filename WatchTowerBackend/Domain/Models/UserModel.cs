using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WatchTowerAPI.Domain.Models;

[Table("Users")]
public class UserModel
{
    [Key]
    public string Login { get; set; }
    
    public string Password { get; set; }

    public ICollection<RoomModel> Rooms { get; set; }
}