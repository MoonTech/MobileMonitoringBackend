using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WatchTowerBackend.Domain.Models;

[Table("Users")]
public class UserModel
{
    [Key]
    public string Login { get; set; }
    
    public string Password { get; set; }
    public string RefreshToken { get; set; }
    public DateTime TokenCreated { get; set; }
    public DateTime TokenExpires { get; set; }

    public ICollection<RoomModel> Rooms { get; set; }
}