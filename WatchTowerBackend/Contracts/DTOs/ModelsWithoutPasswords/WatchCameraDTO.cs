using WatchTowerAPI.Domain.Models;
using WatchTowerBackend.BusinessLogical.Utils;

namespace WatchTowerBackend.Contracts.DTOs.ModelsWithoutPasswords;

public class WatchCameraDTO : CameraDTO
{
    public string WatchUrl
    {
        get
        {
            return Constants.WatchBaseUrl + CameraToken + ".m3u8";
        }
    }
    
    public static implicit operator WatchCameraDTO(CameraModel camera)
    {
        return new WatchCameraDTO()
        {
            Id = camera.Id,
            CameraName = camera.CameraName,
            CameraToken = camera.CameraToken,
            AcceptationState = camera.AcceptationState
        };
    }
}