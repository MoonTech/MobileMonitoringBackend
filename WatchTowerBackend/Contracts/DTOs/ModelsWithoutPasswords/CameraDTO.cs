using WatchTowerAPI.Domain.Models;

namespace WatchTowerBackend.Contracts.DTOs.ModelsWithoutPasswords;

public class CameraDTO
{
    public Guid Id { get; set; }
    public bool? AcceptationState { get; set; }
}