using WatchTowerAPI.Domain.Models;
using System.Collections.Generic;

namespace WatchTowerBackend.Contracts.DTOs.ModelsWithoutPasswords;

// TODO - maybe some inheritance with models without passwords???
public class RoomDTO
{
    public string RoomName { get; set; }
    public IEnumerable<CameraDTO> Cameras { get; set; }
}