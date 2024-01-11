using MobileMonitoringBackend.BusinessLogical.Utils;
using MobileMonitoringBackend.Domain.Models;

namespace MobileMonitoringBackend.Contracts.DTOs.ModelsToDTOs
{
    public class WatchCameraDTO : CameraDTO
    {
        public string WatchUrl
        {
            get
            {
                return Constants.WatchBaseUrl + CameraToken + ".flv";
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
}