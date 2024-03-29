using MobileMonitoringBackend.Domain.Models;

namespace MobileMonitoringBackend.Contracts.DTOs.ModelsToDTOs;

public class CameraDTO
{
    public Guid Id { get; set; }
    public string CameraName { get; set; }
    public string CameraToken { get; set; }
    public bool AcceptationState { get; set; }

    public static implicit operator CameraDTO(CameraModel camera)
    {
        return new CameraDTO()
        {
            Id = camera.Id,
            CameraName = camera.CameraName,
            CameraToken = camera.CameraToken,
            AcceptationState = camera.AcceptationState
        };
    }
}